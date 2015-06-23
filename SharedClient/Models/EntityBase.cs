using SQLite;

namespace KinderChat
{
	/// <summary>
	/// Business entity base class. Provides the ID property.
	/// </summary>
	public abstract class EntityBase
	{
		/// <summary>
		/// Gets or sets the Database ID.
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
	}
}
