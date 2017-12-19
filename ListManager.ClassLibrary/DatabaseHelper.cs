using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite.Net;

namespace ListManager.ClassLibrary
{
    public class DatabaseHelper
    {
        #region ConnectionString

        public static string ConnectionString
        {
            get { return ApplicationData.Current.LocalSettings.Values["ConnectionString"].ToString(); }
            set { ApplicationData.Current.LocalSettings.Values["ConnectionString"] = value; }
        }

        #endregion ConnectionString

        #region Properties and Variables

        // For keeping Connection and Command objects alive instead of
        // using async calls because in many cases async returns too
        // late and the page doesn't get updated with recent changes
        private static SQLiteConnection _SQLiteConn;
        public static SQLiteConnection SQLiteConn
        {
            get
            {
                if (_SQLiteConn == null) { _SQLiteConn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), ConnectionString); }
                return _SQLiteConn;
            }
        }

        private static SQLiteCommand _SQLiteCmd;
        public static SQLiteCommand SQLiteCmd
        {
            get
            {
                if (_SQLiteCmd == null) { _SQLiteCmd = SQLiteConn.CreateCommand(string.Empty); }
                return _SQLiteCmd;
            }
        }
        //////////////////////////////////////////////////////////////

        #endregion Properties and Variables

        #region CreateDatabase

        public async static Task<int> CreateDatabase()
        {
            try
            {
                string SQL = string.Empty;
                int result;

                //////////////////////////////////////////////////////////////////
                // List
                //////////////////////////////////////////////////////////////////
                SQL =
                "CREATE TABLE [List] ( " +
                "   [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                "   [Name] nvarchar(128)  NOT NULL " +
                "); ";

                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                SQL =
                "CREATE INDEX IX_Id_Note ON List(Id); ";
                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                //////////////////////////////////////////////////////////////////
                // ListItem
                //////////////////////////////////////////////////////////////////
                SQL =
                "CREATE TABLE [ListItem] (	" +
                "   [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                "   [ListId] integer NOT NULL REFERENCES List(Id), " +
                "   [Item] nvarchar(128)  NOT NULL, " +
                "   FOREIGN KEY(ListId) REFERENCES List(Id) ON DELETE NO ACTION ON UPDATE NO ACTION " +
                ");";
                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                SQL =
                "CREATE INDEX IX_Id_ListItem ON ListItem(Id); ";
                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                SQL =
                "CREATE INDEX IX_ListId_ListItem ON ListItem(ListId); ";
                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                return result;

            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion CreateDatabase

        #region List Methods

        public static int CreateList(List List)
        {
            try
            {
                string SQL =
                    " INSERT INTO [List] " +
                    "       ([Id] " +
                    "       ,[Name]) " +
                    "     VALUES  " +
                    "       ( null " +
                    "       , '" + List.Name.Replace("'", "''") + "'" +
                    "       )";

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int UpdateList(List List)
        {
            try
            {
                string SQL =
                    "UPDATE  List " +
                    "   SET  Name = '" + List.Name.Trim().Replace("'", "''") + "'" +
                    " WHERE	Id =  " + List.Id;

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static List GetList(int ListId)
        {
            try
            {
                string SQL =
                    "SELECT " +
                    "  [List].Id " +
                    " ,[List].[Name] " +
                    "  FROM	List" +
                    " WHERE [List].Id = " + ListId;

                SQLiteCmd.CommandText = SQL;
                List ListItem = SQLiteCmd.ExecuteQuery<List>().First();

                return ListItem;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int DeleteList(int ListId)
        {
            try
            {
                // Delete all ListItems first
                string SQL =
                    "DELETE FROM ListItem WHERE ListId =  " + ListId;

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                // Then Delete the List itself
                SQL =
                    "DELETE FROM List WHERE Id =  " + ListId;

                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static List<List> GetLists()
        {
            try
            {
                string SQL =
                    "SELECT  List.Id " +
                    "		,List.Name" +
                    " FROM  List  " +
                    "ORDER By List.Id ";

                SQLiteCmd.CommandText = SQL;
                List<List> Lists = SQLiteCmd.ExecuteQuery<List>();

                return Lists;

            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int ClearAllLists()
        {
            try
            {
                string SQL =
                    "DELETE FROM ListItem ";

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                SQL =
                    "DELETE FROM List ";

                SQLiteCmd.CommandText = SQL;
                result = SQLiteCmd.ExecuteNonQuery();

                return result;

            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion List Methods

        #region ListItem Methods

        public static List<ListItem> GetListItems(int ListId)
        {
            try
            {
                string SQL =
                    "SELECT  ListItem.Id " +
                    "		,ListItem.ListId " +
                    "		,List.Name AS List " +
                    "		,ListItem.Item  " +
                    " FROM  List" +
                    "       ,ListItem  " +
                    " WHERE List.Id = " + ListId +
                    "   AND ListItem.ListId = List.Id; ";  

                SQLiteCmd.CommandText = SQL;
                List<ListItem> ListItems = SQLiteCmd.ExecuteQuery<ListItem>();

                return ListItems;

            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static ListItem GetListItem(int ListItemId)
        {
            try
            {
                string SQL =
                    "SELECT " +
                    " [ListItem].Id " +
                    ",[ListItem].[ListId] " +
                    ",[ListItem].[Name] " +
                    "  FROM	ListItem " +
                    "  JOIN [List] ON ListItem.ListId = [List].Id " +
                    " WHERE [ListItem].Id = " + ListItemId;

                SQLiteCmd.CommandText = SQL;
                ListItem ListItem = SQLiteCmd.ExecuteQuery<ListItem>().First();

                return ListItem;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int UpdateListItem(ListItem ListItem)
        {
            try
            {
                string SQL =
                    "UPDATE  ListItem " +
                    "   SET  ListId =  " + ListItem.ListId +
                    "       ,Item = '" + ListItem.Item.Trim().Replace("'", "''") + "'" +
                    " WHERE	Id =  " + ListItem.Id;

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int DeleteListItem(int ListItemId)
        {
            try
            {
                string SQL =
                    "DELETE FROM ListItem WHERE Id =  " + ListItemId;

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int DeleteListItems(int ListId)
        {
            try
            {
                string SQL =
                    "DELETE FROM ListItem WHERE ListId =  " + ListId;

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static int CreateListItem(ListItem ListItem)
        {
            try
            {
                string SQL =
                    " INSERT INTO [ListItem] " +
                    "       ([Id] " +
                    "       ,[ListId] " +
                    "       ,[Item]) " +
                    "     VALUES  " +
                    "       ( null " +
                    "       , " + ListItem.ListId +
                    "       , '" + ListItem.Item.Replace("'", "''") + "'" + ")";

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;
            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        #endregion ListItem Methods

        #region Load Database

        public static int LoadPhoneDatabase()
        {
            try
            {
                string SQL =
                "INSERT INTO List (Id, Name) " +
                "VALUES " +
                " (null, 'Monday') " +
                ",(null, 'Tuesday') " +
                ",(null, 'Wednesday') " +
                ",(null, 'Thursday') " +
                ",(null, 'Friday') " +
                ",(null, 'Saturday') " +
                ",(null, 'Sunday') " +
                ",(null, 'Things to Do') " +
                ",(null, 'Grocery Shopping') " +
                ",(null, 'Household Chores'); ";

                SQLiteCmd.CommandText = SQL;
                int result = SQLiteCmd.ExecuteNonQuery();

                return result;

            }
            catch (SQLiteException SQLiteEx)
            {
                throw SQLiteEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion  Load Database
    }
}
