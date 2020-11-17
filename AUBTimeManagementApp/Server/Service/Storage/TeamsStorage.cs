﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Server.DataContracts;
using Server.Service.Storage;

namespace AUBTimeManagementApp.Service.Storage
{
    class TeamsStorage
    {
        #region Add
        /// <summary>
        /// Adds a new team to the database
        /// </summary>
        /// <returns>The unique teamID of the created team, -1 if it was unsuccessful</returns>
        public static int addTeam(string teamName, string admin, string[] members) {
            return 0;
        }

        /// <summary>
        /// Adds a member to the team
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool addTeamMember(int teamID, string username) {
            return true;
        }

        /// <summary>
        /// Adds an admin to the team (from the existing members)
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool addTeamAdmin(int teamID, string username) {
            return true;
        }
        #endregion

        #region Remove
        /// <summary>
        /// Removes a member from the team
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool removeTeamMember(int teamID, string username) {
            //If the team is now empty, remove team
            return true;
        }

        /// <summary>
        /// Sets an admin to be a usual member
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool removeTeamAdmin(int teamID, string username) {
            return true;
        }
        #endregion

        #region Get

        public static List<int> getUserTeams(string username) {
            try {
                string connectionString = ConnectionUtil.connectionString;
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string query = "SELECT TeamID FROM isMember WHERE Username = @Username";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                SqlDataReader dataReader = command.ExecuteReader();

                List<int> teams = new List<int>();
                while (dataReader.Read()) { teams.Add(dataReader.GetInt32(0)); }

                sqlConnection.Close(); return teams;
            }
            catch (SqlException exception) { Console.WriteLine("getUserTeams: " + exception.Message); throw; }
        }

        /// <summary>
        /// Gets a list of the members of a team
        /// </summary>
        /// <returns>The usernames of each member</returns>
        public static string[] getTeamMembers(int teamID) {
            return new string[] { "q" };
        }

        public static Team getTeamInfo(int teamID) {
            return new Team(0, "testTeam");
        }

        public static List<int> getTeamEvents(int teamID, int priority)
        {
            return null;
        }
        #endregion
    }
}
