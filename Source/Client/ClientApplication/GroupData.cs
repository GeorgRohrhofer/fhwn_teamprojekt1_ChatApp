using System;

/// <summary>
/// Class to hold information on groups. Used to convert server data json to object.
/// </summary>
public class GroupData
{
	/// <summary>
	/// Property that holds the group name.
	/// </summary>
	public string group_name { get; set; }

	/// <summary>
	/// Property that holds the group id.
	/// </summary>
	public int group_id { get; set; }

	/// <summary>
	/// Constructor for a new instance of GroupData.
	/// </summary>
	/// <param name="group_id">GroupID of the group.</param>
	/// <param name="group_name">Name of the group.</param>
	public GroupData(int group_id, string group_name)
	{
		this.group_id = group_id;
		this.group_name = group_name;
	}
}
