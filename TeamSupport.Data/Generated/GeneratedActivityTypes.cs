using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    [Serializable]
    public partial class ActivityType : BaseItem
    {
        private ActivityTypes _ActivityTypes;

        public ActivityType(DataRow row, ActivityTypes ActivityTypes) : base(row, ActivityTypes)
        {
            _ActivityTypes = ActivityTypes;
        }

        #region Properties

        public ActivityTypes Collection
        {
            get { return _ActivityTypes; }
        }




        public int ActivityTypeID
        {
            get { return (int)Row["ActivityTypeID"]; }
        }



        public string Description
        {
            get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
            set { Row["Description"] = CheckValue("Description", value); }
        }



        public int ModifierID
        {
            get { return (int)Row["ModifierID"]; }
            set { Row["ModifierID"] = CheckValue("ModifierID", value); }
        }

        public int CreatorID
        {
            get { return (int)Row["CreatorID"]; }
            set { Row["CreatorID"] = CheckValue("CreatorID", value); }
        }

        public int OrganizationID
        {
            get { return (int)Row["OrganizationID"]; }
            set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
        }

        public int Position
        {
            get { return (int)Row["Position"]; }
            set { Row["Position"] = CheckValue("Position", value); }
        }

        public string Name
        {
            get { return (string)Row["Name"]; }
            set { Row["Name"] = CheckValue("Name", value); }
        }


        /* DateTime */







        public DateTime DateModified
        {
            get { return DateToLocal((DateTime)Row["DateModified"]); }
            set { Row["DateModified"] = CheckValue("DateModified", value); }
        }

        public DateTime DateModifiedUtc
        {
            get { return (DateTime)Row["DateModified"]; }
        }

        public DateTime DateCreated
        {
            get { return DateToLocal((DateTime)Row["DateCreated"]); }
            set { Row["DateCreated"] = CheckValue("DateCreated", value); }
        }

        public DateTime DateCreatedUtc
        {
            get { return (DateTime)Row["DateCreated"]; }
        }


        #endregion


    }

    public partial class ActivityTypes : BaseCollection, IEnumerable<ActivityType>
    {
        public ActivityTypes(LoginUser loginUser) : base(loginUser)
        {
        }

        #region Properties

        public override string TableName
        {
            get { return "ActivityTypes"; }
        }

        public override string PrimaryKeyFieldName
        {
            get { return "ActivityTypeID"; }
        }



        public ActivityType this[int index]
        {
            get { return new ActivityType(Table.Rows[index], this); }
        }


        #endregion

        #region Protected Members

        partial void BeforeRowInsert(ActivityType ActivityType);
        partial void AfterRowInsert(ActivityType ActivityType);
        partial void BeforeRowEdit(ActivityType ActivityType);
        partial void AfterRowEdit(ActivityType ActivityType);
        partial void BeforeRowDelete(int ActivityTypeID);
        partial void AfterRowDelete(int ActivityTypeID);

        partial void BeforeDBDelete(int ActivityTypeID);
        partial void AfterDBDelete(int ActivityTypeID);

        #endregion

        #region Public Methods

        public ActivityTypeProxy[] GetActivityTypeProxies()
        {
            List<ActivityTypeProxy> list = new List<ActivityTypeProxy>();

            foreach (ActivityType item in this)
            {
                list.Add(item.GetProxy());
            }

            return list.ToArray();
        }

        public virtual void DeleteFromDB(int ActivityTypeID)
        {
            SqlCommand deleteCommand = new SqlCommand();
            deleteCommand.CommandType = CommandType.Text;
            deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActivityTypes] WHERE ([ActivityTypeID] = @ActivityTypeID);";
            deleteCommand.Parameters.Add("ActivityTypeID", SqlDbType.Int);
            deleteCommand.Parameters["ActivityTypeID"].Value = ActivityTypeID;

            BeforeDBDelete(ActivityTypeID);
            BeforeRowDelete(ActivityTypeID);
            TryDeleteFromDB(deleteCommand);
            AfterRowDelete(ActivityTypeID);
            AfterDBDelete(ActivityTypeID);
        }

        public override void Save(SqlConnection connection)
        {
            //SqlTransaction transaction = connection.BeginTransaction("ActivityTypesSave");
            SqlParameter tempParameter;
            SqlCommand updateCommand = connection.CreateCommand();
            updateCommand.Connection = connection;
            //updateCommand.Transaction = transaction;
            updateCommand.CommandType = CommandType.Text;
            updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActivityTypes] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([ActivityTypeID] = @ActivityTypeID);";


            tempParameter = updateCommand.Parameters.Add("ActivityTypeID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 255;
                tempParameter.Scale = 255;
            }

            tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 255;
                tempParameter.Scale = 255;
            }

            tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 23;
                tempParameter.Scale = 23;
            }

            tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }


            SqlCommand insertCommand = connection.CreateCommand();
            insertCommand.Connection = connection;
            //insertCommand.Transaction = transaction;
            insertCommand.CommandType = CommandType.Text;
            insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActivityTypes] (    [Name],    [Description],    [Position],    [OrganizationID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @Name, @Description, @Position, @OrganizationID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";


            tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 23;
                tempParameter.Scale = 23;
            }

            tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 23;
                tempParameter.Scale = 23;
            }

            tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 10;
                tempParameter.Scale = 10;
            }

            tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 255;
                tempParameter.Scale = 255;
            }

            tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
            if (tempParameter.SqlDbType == SqlDbType.Float)
            {
                tempParameter.Precision = 255;
                tempParameter.Scale = 255;
            }


            insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
            SqlCommand deleteCommand = connection.CreateCommand();
            deleteCommand.Connection = connection;
            //deleteCommand.Transaction = transaction;
            deleteCommand.CommandType = CommandType.Text;
            deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActivityTypes] WHERE ([ActivityTypeID] = @ActivityTypeID);";
            deleteCommand.Parameters.Add("ActivityTypeID", SqlDbType.Int);

            try
            {
                foreach (ActivityType ActivityType in this)
                {
                    if (ActivityType.Row.RowState == DataRowState.Added)
                    {
                        BeforeRowInsert(ActivityType);
                        for (int i = 0; i < insertCommand.Parameters.Count; i++)
                        {
                            SqlParameter parameter = insertCommand.Parameters[i];
                            if (parameter.Direction != ParameterDirection.Output)
                            {
                                parameter.Value = ActivityType.Row[parameter.ParameterName];
                            }
                        }

                        if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
                        if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

                        insertCommand.ExecuteNonQuery();
                        Table.Columns["ActivityTypeID"].AutoIncrement = false;
                        Table.Columns["ActivityTypeID"].ReadOnly = false;
                        if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
                            ActivityType.Row["ActivityTypeID"] = (int)insertCommand.Parameters["Identity"].Value;
                        AfterRowInsert(ActivityType);
                    }
                    else if (ActivityType.Row.RowState == DataRowState.Modified)
                    {
                        BeforeRowEdit(ActivityType);
                        for (int i = 0; i < updateCommand.Parameters.Count; i++)
                        {
                            SqlParameter parameter = updateCommand.Parameters[i];
                            parameter.Value = ActivityType.Row[parameter.ParameterName];
                        }
                        if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
                        if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

                        updateCommand.ExecuteNonQuery();
                        AfterRowEdit(ActivityType);
                    }
                    else if (ActivityType.Row.RowState == DataRowState.Deleted)
                    {
                        int id = (int)ActivityType.Row["ActivityTypeID", DataRowVersion.Original];
                        deleteCommand.Parameters["ActivityTypeID"].Value = id;
                        BeforeRowDelete(id);
                        deleteCommand.ExecuteNonQuery();
                        AfterRowDelete(id);
                    }
                }
                //transaction.Commit();
            }
            catch (Exception)
            {
                //transaction.Rollback();
                throw;
            }
            Table.AcceptChanges();
            if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        }

        public void BulkSave()
        {

            foreach (ActivityType ActivityType in this)
            {
                if (ActivityType.Row.Table.Columns.Contains("CreatorID") && (int)ActivityType.Row["CreatorID"] == 0) ActivityType.Row["CreatorID"] = LoginUser.UserID;
                if (ActivityType.Row.Table.Columns.Contains("ModifierID")) ActivityType.Row["ModifierID"] = LoginUser.UserID;
            }

            SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
            copy.BulkCopyTimeout = 0;
            copy.DestinationTableName = TableName;
            copy.WriteToServer(Table);

            Table.AcceptChanges();

            if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        }

        public ActivityType FindByActivityTypeID(int ActivityTypeID)
        {
            foreach (ActivityType ActivityType in this)
            {
                if (ActivityType.ActivityTypeID == ActivityTypeID)
                {
                    return ActivityType;
                }
            }
            return null;
        }

        public virtual ActivityType AddNewActivityType()
        {
            if (Table.Columns.Count < 1) LoadColumns("ActivityTypes");
            DataRow row = Table.NewRow();
            Table.Rows.Add(row);
            return new ActivityType(row, this);
        }

        public virtual void LoadByActivityTypeID(int ActivityTypeID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SET NOCOUNT OFF; SELECT [ActivityTypeID], [Name], [Description], [Position], [OrganizationID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[ActivityTypes] WHERE ([ActivityTypeID] = @ActivityTypeID);";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("ActivityTypeID", ActivityTypeID);
                Fill(command);
            }
        }

        public static ActivityType GetActivityType(LoginUser loginUser, int ActivityTypeID)
        {
            ActivityTypes ActivityTypes = new ActivityTypes(loginUser);
            ActivityTypes.LoadByActivityTypeID(ActivityTypeID);
            if (ActivityTypes.IsEmpty)
                return null;
            else
                return ActivityTypes[0];
        }




        public void LoadByPosition(int organizationID, int position)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ActivityTypes WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                command.Parameters.AddWithValue("Position", position);
                Fill(command);
            }
        }

        public void LoadAllPositions(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ActivityTypes WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void ValidatePositions(int organizationID)
        {
            ActivityTypes ActivityTypes = new ActivityTypes(LoginUser);
            ActivityTypes.LoadAllPositions(organizationID);
            int i = 0;
            foreach (ActivityType ActivityType in ActivityTypes)
            {
                ActivityType.Position = i;
                i++;
            }
            ActivityTypes.Save();
        }

        public void MovePositionUp(int ActivityTypeID)
        {
            ActivityTypes types1 = new ActivityTypes(LoginUser);
            types1.LoadByActivityTypeID(ActivityTypeID);
            if (types1.IsEmpty || types1[0].Position < 1) return;

            ActivityTypes types2 = new ActivityTypes(LoginUser);
            types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position - 1);
            if (!types2.IsEmpty)
            {
                types2[0].Position = types2[0].Position + 1;
                types2.Save();
            }

            types1[0].Position = types1[0].Position - 1;
            types1.Save();
            ValidatePositions(LoginUser.OrganizationID);
        }

        public void MovePositionDown(int ActivityTypeID)
        {
            ActivityTypes types1 = new ActivityTypes(LoginUser);
            types1.LoadByActivityTypeID(ActivityTypeID);
            if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

            ActivityTypes types2 = new ActivityTypes(LoginUser);
            types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position + 1);
            if (!types2.IsEmpty)
            {
                types2[0].Position = types2[0].Position - 1;
                types2.Save();
            }

            types1[0].Position = types1[0].Position + 1;
            types1.Save();

            ValidatePositions(LoginUser.OrganizationID);
        }


        public virtual int GetMaxPosition(int organizationID)
        {
            int position = -1;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT MAX(Position) FROM ActivityTypes WHERE OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("OrganizationID", organizationID);
                object o = ExecuteScalar(command);
                if (o == DBNull.Value) return -1;
                position = (int)o;
            }
            return position;
        }



        #endregion

        #region IEnumerable<ActivityType> Members

        public IEnumerator<ActivityType> GetEnumerator()
        {
            foreach (DataRow row in Table.Rows)
            {
                yield return new ActivityType(row, this);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
