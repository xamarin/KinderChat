using System;
using System.ComponentModel;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Koush;
using KinderChat.Converters;
using KinderChat.ServerClient;
using KinderChat.ViewModels.Messages;

namespace KinderChat
{
	[Activity(Label = "Conversation", ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class ConversationActivity : BaseActivity
    {
        ListView messagesListView;
        ImageButton sendButton;
        EditText inputText;
		ConversationViewModel viewModel;
	    private TextView typingText;
	    private ImageButton attachButton;
	    private const int SelectPhotoId = 100;

	    public const string RecipientId = "recipient";

        protected override int LayoutResource
        {
            get { return Resource.Layout.activity_conversation_messages; }
        }

        protected override void OnCreate(Bundle bundle)
        {
			base.OnCreate(bundle);

			var id = Intent.GetLongExtra (RecipientId, 0);
			viewModel = new ConversationViewModel (id);
           
            messagesListView = FindViewById<ListView>(Resource.Id.messages_list_view);

            //TODO: use RecycleView
			var adapter = new ObservableAdapaterBase<MessageViewModel>(ApplicationContext, viewModel.Messages,
                OnMessageTemplate, 
                OnMessageViewCreate);
            adapter.AssignListView(messagesListView);

            sendButton = FindViewById<ImageButton>(Resource.Id.send_button);
            attachButton = FindViewById<ImageButton>(Resource.Id.attach_button);
            typingText = FindViewById<TextView>(Resource.Id.typing_text);
            inputText = FindViewById<EditText>(Resource.Id.input_text);
            inputText.AfterTextChanged += InputTextChanged;

            sendButton.Click += OnSendClick;
            attachButton.Click += OnAttachClick;
			viewModel.PropertyChanged += ViewModel_PropertyChanged;
			viewModel.ExecuteLoadMessagesCommand ();
        }

	    private void OnAttachClick(object sender, EventArgs e)
	    {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Select a photo"), SelectPhotoId);
	    }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent imageReturnedIntent)
	    {
	        switch (requestCode)
	        {
                case SelectPhotoId:
	                if (resultCode == Result.Ok)
	                {
                        Stream stream = ContentResolver.OpenInputStream(imageReturnedIntent.Data);
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);
	                    ms.Position = 0;
                        viewModel.SendMessage("", ms.ToArray());
	                }
                break;
	        }
			base.OnActivityResult(requestCode, resultCode, imageReturnedIntent);
	    }

	    private void InputTextChanged(object sender, AfterTextChangedEventArgs e)
	    {
            viewModel.HandleTyping(inputText.Text);
	    }

	    void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
			RunOnUiThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					if (viewModel.IsBusy)
						AndHUD.Shared.Show (this, Resources.GetString(Resource.String.loading), maskType: MaskType.Clear);
					else {
						AndHUD.Shared.Dismiss (this);
						if(viewModel.Friend != null)
							SupportActionBar.Title = viewModel.Friend.Name;
					}
                    break;

                case ConversationViewModel.IsTypingPropertyName:
				        if (!string.IsNullOrEmpty(viewModel.Friend.Name))
				        {
				            typingText.Text = string.Format("{0} is typing...", viewModel.Friend.Name);
				            typingText.Visibility = viewModel.IsTyping ? ViewStates.Visible : ViewStates.Invisible;
				        }
					break;
				}
			});
        }

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.OnClose();
		}

        private int OnMessageTemplate(int position, MessageViewModel msg)
        {
            if (msg is ImageMessageViewModel)
            {
                return !msg.IsIncoming ? Resource.Layout.item_conversation_msg_image_outgoing : Resource.Layout.item_conversation_msg_image_icoming;
            }
            return !msg.IsIncoming ? Resource.Layout.item_conversation_msg_outgoing : Resource.Layout.item_conversation_msg_icoming;
        }

        //TODO: use RecycleView
        private void OnMessageViewCreate(int position, View view, MessageViewModel msgVm)
        {
            var textVm = msgVm as TextMessageViewModel;
            var imageVm = msgVm as ImageMessageViewModel;

            var msgDate = view.FindViewById<TextView>(Resource.Id.msg_timestamp);
            msgDate.Text = new TimestampConverter().Convert(msgVm.Timestamp);

            if (!msgVm.IsIncoming)
            {
                var msgStatus = view.FindViewById<ImageView>(Resource.Id.msg_status);
                switch (msgVm.Status)
                {
                    case MessageStatus.Unsent:
                        msgStatus.SetImageResource(Resource.Drawable.msg_status_sending);
                        break;
                    case MessageStatus.Sent:
                        msgStatus.SetImageResource(Resource.Drawable.msg_status_sent);
                        break;
                    case MessageStatus.Delivered:
                        msgStatus.SetImageResource(Resource.Drawable.msg_status_delivered);
                        break;
                    case MessageStatus.Seen:
                        msgStatus.SetImageResource(Resource.Drawable.msg_status_seen);
                        break;
                }
            }

            if (imageVm != null)
            {
                var thumbnailView = view.FindViewById<ImageView>(Resource.Id.thumnail_image);
                thumbnailView.SetImageBitmap(BitmapFactory.DecodeByteArray(imageVm.Thumbnail, 0, imageVm.Thumbnail.Length));
            }
            else if (textVm != null)
            {
                var msgText = view.FindViewById<TextView>(Resource.Id.msg_text);
                msgText.Text = textVm.Text;
            }

			var msgImage = view.FindViewById<ImageView> (Resource.Id.msg_photo);

			var photoUrl = string.Empty;
            if (msgVm.IsIncoming)
                photoUrl = viewModel.Friend.Photo;
            else
                photoUrl = EndPoints.BaseUrl + Settings.Avatar;
			
			UrlImageViewHelper.SetUrlDrawable (msgImage, photoUrl, Resource.Drawable.default_photo);
        }

        private void OnSendClick(object sender, EventArgs e)
        {
            string text = inputText.Text;
            if (!string.IsNullOrEmpty(text))
            {
				viewModel.SendMessage (text, null);
                inputText.Text = string.Empty;
            }
        }
    }
}