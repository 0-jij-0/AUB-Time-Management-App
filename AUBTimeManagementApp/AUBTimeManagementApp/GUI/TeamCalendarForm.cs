﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;
using AUBTimeManagementApp.DataContracts;

namespace AUBTimeManagementApp.GUI {
    public partial class TeamCalendarForm : Form {
        List<CalendarItem> _items = new List<CalendarItem>();
        public Team team { get; set; }
        public bool mergedCalendarShown { get; set; }
        AddEvent addEventForm;
        bool isAdmin;

        public TeamCalendarForm(Team _team, bool merged) {
            Client.Client.Instance.setForm(this);
            InitializeComponent();

            team = _team;
            teamNameLabel.Text = team.teamName;
            isAdmin = team.isAdmin(Client.Client.Instance.username);
            if (!isAdmin) {
                merged = false;
                addEventButton.Enabled = false;
                calendarTypeButton.Enabled = false;
            }

            mergedCalendarShown = !merged;
            changeCalendarType();
        }

        private void monthView_SelectionChanged(object sender, EventArgs e) {
            if (mergedCalendarShown) {
                DateTime start = monthView.SelectionStart, end = monthView.SelectionEnd;
                if (end.Subtract(start).Days > 6) {
                    end = start; end.AddDays(7);
                    monthView.SelectionEnd = end;
                }
            }
            calendar.SetViewRange(monthView.SelectionStart, monthView.SelectionEnd);
        }

        private void addEvent_Click(object sender, EventArgs e) {
            if (addEventForm != null && addEventForm.Visible) {
                addEventForm.Focus();
                return;
            }
            addEventForm = new AddEvent(this);
            addEventForm.Show();
        }

        private void calendarTypeButton_Click(object sender, EventArgs e) {
            changeCalendarType();
        }

        private void backButton_Click(object sender, EventArgs e) {
            TeamDetailsForm newForm = new TeamDetailsForm(team);
            newForm.Show(); Close();
        }

        /// <summary>
        /// Swaps between team calendar and merged calendar for the team
        /// </summary>
        private void changeCalendarType() {
            mergedCalendarShown = !mergedCalendarShown;
            if (mergedCalendarShown) { 
                calendarTypeButton.Text = "Show Team Calendar";
                DateTime start = monthView.SelectionStart, end = monthView.SelectionEnd;
                if (end.Subtract(start).Days > 6) { 
                    end = start; end.AddDays(7);
                    monthView.SelectionEnd = end;
                }

                //TODO: Fix priority threshold
                Client.Client.Instance.GetMergedTeamSchedule(team.teamID, start, end, 3);
            }
            else {
                calendar.Items.Clear(); _items.Clear();
                calendarTypeButton.Text = "Show Merged Schedule";
                Client.Client.Instance.GetTeamSchedule(team.teamID);
            }
        }

        public void displayColorFreq(double [,] freq) {
            int n = freq.GetLength(0), m = freq.GetLength(1);
            int R = 255;

            DateTime start = monthView.SelectionStart, end = monthView.SelectionEnd; //Maybe take those from the server
            calendar.Items.Clear(); _items.Clear();
            for (int i = 0; i < n; i++) {
                for(int j = 0; j < m; j++) {
                    CalendarItem curItem = new CalendarItem(calendar);
                    DateTime curS = start; curS.AddDays(i); curS.AddMinutes(j);
                    curItem.StartDate = curS;
                    curS.AddMinutes(1);
                    curItem.EndDate = curS;
                    curItem.ApplyColor(Color.FromArgb((int)(255 * freq[i, j]), 0, 0));
                }
            }
        }

        public void displayEvent(Event _event) {
            CalendarItem curEvent = new CalendarItem(calendar, _event.startTime, _event.endTime, _event.eventName, _event.ID, _event.priority);
            calendar.Items.Add(curEvent);
            _items.Add(curEvent);
        }
    }
}
