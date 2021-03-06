﻿using AUBTimeManagementApp.DataContracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;

namespace AUBTimeManagementApp.GUI
{
    public partial class mainForm : Form {
        List<CalendarItem> _items = new List<CalendarItem>();
        string _username;
        int selectedItemID;
        CalendarItem selectedItem;
        AddEvent addEventForm;
        InvitationsForm invitationsForm;
        public mainForm(string username = null) {
            Client.Client.Instance.setForm(this);
            _username = username;
            InitializeComponent();
            calendar.AllowItemEdit = false;
            calendar.AllowItemResize = false;
            label1.Text = "Welcome " + username + "!";
            label1.Show();
            filteringPanel.Hide();
            eventDetailsPanel.Hide();
            Low.Checked = true;
            Medium.Checked = true;
            High.Checked = true;
            updateInvitationNotification(Client.Client.Instance.GetInvitations().Count);

            monthView.SelectionStart = DateTime.Today;
            monthView.SelectionEnd = DateTime.Today.AddDays(2);
        }

        /// <summary>
        /// Displays the event on the calendar
        /// </summary>
        public void displayEvent(int eventID, string eventName, int priority, DateTime startDate, DateTime endDate, bool teamEvent, string Link) {
            CalendarItem curEvent = new CalendarItem(calendar, startDate, endDate, eventName, eventID, priority, teamEvent, Link);
            calendar.Items.Add(curEvent);
            _items.Add(curEvent);

            if (priority == 1) { curEvent.BackgroundColor = curEvent.BackgroundColorLighter = Color.LightBlue; }
            else if (priority == 2) { curEvent.BackgroundColor = curEvent.BackgroundColorLighter = Color.LightGreen; }
            else { curEvent.BackgroundColor = curEvent.BackgroundColorLighter = Color.PaleVioletRed; }
        }


        /// <summary>
        /// Opens the teams panel
        /// </summary>
        private void TeamButton_Click(object sender, EventArgs e) {
            TeamsForm addTeamWindow = new TeamsForm();
            addTeamWindow.Show();
            Close();
        }

        /// <summary>
        /// Opens the add event panel
        /// </summary>
        private void addEvent_MouseClick(object sender, MouseEventArgs e) {
            if (addEventForm != null && addEventForm.Visible) {
                addEventForm.Focus();
                return;
            }
            addEventForm = new AddEvent(this);
            addEventForm.Show();
        }

        /// <summary>
        /// Shows all the items that appear in the days shown 
        /// </summary>
        private void PlaceItems() {
            foreach (CalendarItem item in _items) {
                if (calendar.ViewIntersects(item)) {
                    calendar.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Shows the days on the calendar according to the monthView component
        /// </summary>
        private void monthView_SelectionChanged(object sender, EventArgs e) {
            calendar.SetViewRange(monthView.SelectionStart, monthView.SelectionEnd);
        }

        /// <summary>
        /// Loads all the items on the calendar
        /// </summary>
        private void calendar_LoadItems(object sender, CalendarLoadEventArgs e) {
            PlaceItems();
        }

        /// <summary>
        /// When we double click on the calendar, if there is an event there we open its description
        /// Otherwise, we open the add event panel set at the appropriate time
        /// </summary>
        private void calendar_ItemDoubleClick(object sender, CalendarItemEventArgs e) {
            selectedItem = e.Item;
            if(selectedItem.Selected)
			{
                mainPanel.Hide();
                eventDetailsPanel.Show();
                // Display Selected Event Details
                detailsEventName.Text = selectedItem.Text;
                datePickerStart.Value = selectedItem.StartDate.Date;
                timePickerStart.Value = selectedItem.StartDate.Date + selectedItem.StartDate.TimeOfDay;
                datePickerEnd.Value = selectedItem.EndDate.Date;
                timePickerEnd.Value = selectedItem.EndDate.Date + selectedItem.EndDate.TimeOfDay;
                ModifyPriority.Value = selectedItem.priority;
                LinkBox.Text = selectedItem.Link;
                selectedItemID = selectedItem.eventID;

                if(selectedItem.teamEvent)
				{
                    detailsEventName.ReadOnly = true;
                    datePickerStart.Enabled= false;
                    timePickerStart.Enabled = false;
                    datePickerEnd.Enabled = false;
                    timePickerEnd.Enabled = false;
                    LinkBox.ReadOnly = true;
                    ModifyEventBut.Text = "Modify Priority";
                    eventTypeText.Text = "Team Event";
				}
				else
				{
                    detailsEventName.ReadOnly = false;
                    datePickerStart.Enabled = true;
                    timePickerStart.Enabled = true;
                    datePickerEnd.Enabled = true;
                    timePickerEnd.Enabled = true;
                    LinkBox.ReadOnly = false;
                    ModifyEventBut.Text = "Modify";
                    eventTypeText.Text = "Personal Event";
                }
            }
            else
            {
                AddEvent addEventWindow = new AddEvent(this, e.Item.StartDate, e.Item.EndDate);
                calendar.Items.Remove(e.Item);
                addEventWindow.Show();
            }
        }

        /// <summary>
        /// Go back to the sign in/up form
        /// </summary>
        private void backButton_Click(object sender, EventArgs e) {
            Client.Client.Instance.logOut();
            SignInUpForm nF = new SignInUpForm();
            nF.Show();
            Close();
        }

        /// <summary>
        /// Show the filtering panel and hides the main one
        /// </summary>
		private void filterUserScheduleBut_Click(object sender, EventArgs e)
		{
            mainPanel.Hide();
            filteringPanel.Show();
        }

        /// <summary>
        /// Hides the filtering panel and shows the main one
        /// </summary>
		private void filterBackBut_Click(object sender, EventArgs e)
		{
            filteringPanel.Hide();
            mainPanel.Show();
		}

        /// <summary>
        /// Fetches the filtered calendar from the server
        /// </summary>
		private void filterDoneButton_Click(object sender, EventArgs e)
		{
            filteringPanel.Hide();
            mainPanel.Show();
            _items.Clear();
            calendar.Items.Clear();
            Client.Client.Instance.FilterUserSchedule(Low.Checked, Medium.Checked, High.Checked );
        }

        /// <summary>
        /// Hides the event details
        /// </summary>
		private void eventDetailsBackBut_Click(object sender, EventArgs e)
		{
            eventDetailsPanel.Hide();
            mainPanel.Show();
		}

        /// <summary>
        /// Submits a request to the server to delete an event
        /// </summary>
		private void DeleteEventBut_Click(object sender, EventArgs e)
		{
            //Confirm that the user wnats to delete the event.
            var result = MessageBox.Show("Are you sure you would like to delete this event from your schedule?",
                "Delete Event", MessageBoxButtons.YesNo);
            //If the yes button is pressed delete event
            if (result == DialogResult.Yes)
            { 
               Client.Client.Instance.CancelUserEvent(selectedItemID, selectedItem.teamEvent);
               eventDetailsPanel.Hide();
                mainPanel.Show();
                _items.Remove(selectedItem);
                calendar.Items.Remove(selectedItem);
            }
        }
        /// <summary>
        /// Submits a request to the server to modify an event
        /// </summary>
		private void ModifyEventBut_Click(object sender, EventArgs e)
		{
            //Confirm that the user wnats to delete the event.
            var result = MessageBox.Show("Are you sure you would like to save the changes to this event?",
                "Modify Event", MessageBoxButtons.YesNo);
            //If the yes button is pressed delete event
            if (result == DialogResult.Yes)
            {
                DateTime startDate = datePickerStart.Value.Date + timePickerStart.Value.TimeOfDay;
                DateTime endDate = datePickerEnd.Value.Date + timePickerEnd.Value.TimeOfDay;
                Event updatedEvent = new Event(selectedItem.eventID,ModifyPriority.Value, _username, detailsEventName.Text, startDate, endDate, selectedItem.teamEvent, LinkBox.Text);
                _items.Remove(selectedItem);
                calendar.Items.Remove(selectedItem);
                displayEvent(updatedEvent.ID, updatedEvent.eventName, updatedEvent.priority, updatedEvent.startTime, updatedEvent.endTime, updatedEvent.teamEvent, updatedEvent.link);
                selectedItem = _items[_items.Count - 1];
                monthView_SelectionChanged(null, null);
                Client.Client.Instance.ModifyUserEvent(updatedEvent);
            }
        }

        /// <summary>
        /// Refreshes the calendar so that it displays all the events
        /// </summary>
        private void Refresh_Click(object sender, EventArgs e)
        {
            Low.Checked = true;
            Medium.Checked = true;
            High.Checked = true;
            calendar.Items.Clear(); _items.Clear();
            Client.Client.Instance.GetUserSchedule();
            monthView_SelectionChanged(null, null);
        }

        /// <summary>
        /// Opens the invitations form
        /// </summary>
        private void InvitationsButton_Click(object sender, EventArgs e)
        {
            if (invitationsForm != null && invitationsForm.Visible)
            {
                invitationsForm.Focus();
                return;
            }
            invitationsForm = new InvitationsForm(this);
            invitationsForm.Show();
        }

        /// <summary>
        /// Updates the number representing pending invitations
        /// </summary>
        /// <param name="n">The number of pending invitations</param>
        public void updateInvitationNotification(int n) {
            if (n == 0) {
                invitationsNum.Hide();
            }
            else {
                invitationsNum.Show();
                invitationsNum.Text = n.ToString();
            }
        }

        /// <summary>
        /// Notifies the user about the events that conflict with the newly created event
        /// </summary>
        ///The entire event object (not just a name) although we only used the name
        /// because we might display event details in the warning message
        /// simply because an event name is not a unique identifier of an event
        /// <param name="_event">The newly created event</param>
        /// <param name="conflictingEvents">A list of conflicting events</param>
        public void informUserAboutConflicts(Event _event, List<Event> conflictingEvents)
        {
            string warning = "Warning:\n Event " + _event.eventName + " is in conflict with the following events:\n";
            for (int i = 0; i < conflictingEvents.Count; i++)
            {
                warning += conflictingEvents[i].eventName;
                if (i != conflictingEvents.Count - 1)
                {
                    warning += " & ";
                }
            }

            MessageBox.Show(warning, "Conflict Warning!");
        }
    }
}