using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace KinderChat
{
	public class PointsViewModel : BaseViewModel
	{
		readonly IDataManager dataManager = App.DataManager;

		public string LoadingMessage { get; set; }

		readonly SemaphoreSlim taskSemaphore = new SemaphoreSlim (1);
		readonly BatchUpdateObservableCollectoin<KinderTask> tasks = new BatchUpdateObservableCollectoin<KinderTask>();

		public BatchUpdateObservableCollectoin<KinderTask> Tasks {
			get {
				return tasks;
			}
		}

		public SemaphoreSlim TaskSemaphore {
			get {
				return taskSemaphore;
			}
		}

		int points = -1;
		public const string PointsPropertyName = "Points";

		public int Points {
			get {
				if (points < 0)
					points = Settings.KinderPoints;
				return points;
			}
			set {
				Settings.KinderPoints = value;
				SetProperty (ref points, value, PointsPropertyName);
			}
		}

		int pendingPoints = -1;
		public const string PendingPointsName = "PendingPoints";

		public int PendingPoints {
			get {
				if (pendingPoints < 0)
					pendingPoints = Settings.KinderPointsPending;

				return pendingPoints;
			}
			set {
				Settings.KinderPointsPending = value;
				SetProperty (ref pendingPoints, value, PendingPointsName);
			}
		}

		ICommand loadKinderTasks;

		public ICommand LoadKinderTasks {
			get {
				return loadKinderTasks ?? (loadKinderTasks = new RelayCommand (() => {
				}));
			}
		}

		public async Task ExecuteLoadKinderTasksCommand ()
		{
			if (IsBusy)
				return;

			LoadingMessage = "Loading tasks...";

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("LoadingTasks")) {
					try {
						var kinderTasks = await dataManager.GetKinderTaskAsync ().ConfigureAwait(false);
						await LoadToCollection (kinderTasks);
						await RecalculatePoints ().ConfigureAwait (false);
					} catch (Exception ex) {
						App.Logger.Report (ex);
						RaiseError ("Failed to load tasks");
					}
				}
			}
		}

		async Task RecalculatePoints ()
		{
			await TaskSemaphore.WaitAsync ().ConfigureAwait (false);

			int pending = 0;
			foreach (var t in Tasks) {
				if (!t.IsPending)
					continue;

				pending += t.Points;
				t.IsPending = false;
				await dataManager.AddOrSaveKinderTaskAsync (t).ConfigureAwait (false);
			}

			TaskSemaphore.Release ();

			Points = Points + pending;
			PendingPoints = 0;
		}

		async Task LoadToCollection (IEnumerable<KinderTask> items)
		{
			await TaskSemaphore.WaitAsync ().ConfigureAwait (false);

			using (Tasks.UpdatesBlock ()) {
				Tasks.Clear ();
				Tasks.AddRange (items);
			}

			TaskSemaphore.Release ();
		}

		public void Apply (KinderTask task)
		{
			if (task.IsPending)
				PendingPoints = PendingPoints - task.Points;
			else
				PendingPoints = PendingPoints + task.Points;

			task.IsPending = !task.IsPending;
			dataManager.AddOrSaveKinderTaskAsync (task);
		}
	}
}

