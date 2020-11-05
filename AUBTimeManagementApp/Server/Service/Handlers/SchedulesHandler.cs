﻿using AUBTimeManagementApp.Service.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Server.DataContracts;


namespace Server.Service.Handlers {
    class SchedulesHandler {

        public static int[] getEventList(string userID) {
            //If schedule exists get a list of event IDs
            int[] e = new int[] { };
            if (SchedulesStorage.ScheduleExists(userID))
			{
                e = SchedulesStorage.GetSchedule(userID);
            }
            //else create empty schedule
            else
			{
                SchedulesStorage.CreateSchedule(userID);   
            }
            return e;
        }

        public Schedule getTeamSchedule(int teamID) {
            Schedule teamSchedule = new Schedule(false, "", teamID);

            //Get a list of event IDs
            //Get the detail about each event and add it to a schedule

            return teamSchedule;
        }

        public Schedule getFilteredSchedule(string username, int priority) {
            //Get list of event IDs from the schedule storage
            //Get event details from the event handler
            //Filter the events with the filtering handler
            //Return filtered schedule

            return null;
        }

        public bool addEventToList(string username, int eventID) { 
            //gets username and event ID
            //when user creates an event add it to his schedule
            return false;
		}

        public bool removeEventFromList(string username, int eventID) {
            //gets username abd event ID
            //when user cancels event remove it from his schedule
            return false;
		}

        /// <summary>
        /// Merges many schedules over 1 week
        /// </summary>
        /// <param name="membersSchedule"> Schedules to merge </param>
        /// <param name="startDate"> First Day of the week to be merged </param>
        /// <param name="endDate"> Last Day of the week to be merged </param>
        /// <param name="priorityThreshold">Threshold priority of events to be considered </param>
        /// <returns> A table showing how many events overlap at any given time int that week </returns>
        private int[,] mergeSchedules(List<Schedule> membersSchedule, DateTime startDate, DateTime endDate, int priorityThreshold) {
            int[,] mergedSchedule = new int[7, 24 * 60 + 1];
            foreach (Schedule curSchedule in membersSchedule) {
                int i = 0;
                for (DateTime curDate = startDate; curDate.CompareTo(endDate) <= 0; curDate.AddDays(1), i++) {
                    int day = curDate.Day, month = curDate.Month, year = curDate.Year;
                    List<Event> events = curSchedule.getDailyEvent(day, month, year);

                    foreach (Event curEvent in events) {
                        if (curEvent.getPriority() < priorityThreshold) { continue; }
                        DateTime eventStart = curEvent.getStart();
                        DateTime eventEnd = curEvent.getEnd();
                        int startHour = eventStart.Hour, startMinute = eventStart.Minute;
                        int endHour = eventEnd.Hour, endMinute = eventEnd.Minute;

                        int startIndex = 60 * startHour + startMinute;
                        int endIndex = 60 * endHour + endMinute;
                        mergedSchedule[i, startIndex]++;
                        mergedSchedule[i, endIndex + 1]--;
                    }
                }
            }

            for (int i = 0; i < 7; i++)
                for (int j = 1; j < 24 * 60; j++)
                    mergedSchedule[i, j] += mergedSchedule[i, j - 1];

            return mergedSchedule;
        }

        /// <summary>
        /// Finds free time slots in multiple schedules
        /// <para> </para>
        /// </summary>
        /// <param name="membersSchedule"> Schedules to be merge </param>
        /// <param name="startDate"> First day of the week to be considered </param>
        /// <param name="endDate"> Last day of the week to be considered </param>
        /// <param name="startTime">Starting time where results are needed </param>
        /// <param name="endTime"> Final time where results are needed </param>
        /// <param name="countThreshold"> Threshold count for a time slot to be considered occupied (not free)</param>
        /// <param name="priorityThreshold"> Threshold priority of events to be considered when merging schedules </param>
        /// <returns> A table showing the availability of each time slot (1->free; 0->notFree)</returns>
        public bool[,] getFreeTime(List<Schedule> membersSchedule, DateTime startDate, DateTime endDate, 
            DateTime startTime, DateTime endTime, int countThreshold, int priorityThreshold) {
            int[,] mergedSchedule = mergeSchedules(membersSchedule, startDate, endDate, priorityThreshold);
            bool[,] result = new bool[7, 24 * 60];

            int start = 60 * startTime.Hour + startTime.Minute;
            int end = 60 * endTime.Hour + endTime.Minute;

            for(int i = 0; i < 7; i++) {
                int j = start, k = start;
                while(k != end + 1) {
                    if(mergedSchedule[i, j] >= countThreshold) { j++; k++; continue; }
                    if(mergedSchedule[i, k] < countThreshold) { k++; continue; }

                    while(j != k) { result[i, j] = true; j++; }
                }
                while (j != k) { result[i, j] = true; j++; }
            }

            return result;
        }
    }
}