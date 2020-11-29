﻿using AUBTimeManagementApp.Service.Storage;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Server.DataContracts;
using Server.Service.ControlBlocks;

namespace Server.Service.Handlers
{
    public class EventsHandler : IEventsHandler   {

        public EventsStorage _eventsStorage;
        public EventsHandler()
        {
            _eventsStorage = new EventsStorage();
        }
        // TODO: Modify
        public List<int> getFilteredUserEvents(string username, bool low, bool mid, bool high)
        {
            List<int> events = new List<int>();
            if (low) { events.AddRange(EventsStorage.getFilteredUserEvents(username, 1)); }
            if (mid) { events.AddRange(EventsStorage.getFilteredUserEvents(username, 2)); }
            if (high) { events.AddRange(EventsStorage.getFilteredUserEvents(username, 3)); }
            return events;
        }



        /* Incomplete ! */
        /// <summary>
        /// add an event to the events table and return a list of conflicting events in case of conflict
        /// </summary>
        /// <param name="username"></param>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        public void CreateEvent(Event newEvent)
        {
            _eventsStorage.AddEvent(newEvent);
        }


        /// <summary>
        /// delete an event from the events table
        /// </summary>
        /// <param name="eventId"></param>
        public void CancelEvent(int eventId)
        {
            RemoveEventFromUniversalEventsDB(eventId);
        }

  
        public void RemoveUserFromEventAttendees(int eventID, string username)
        {
            /*// get event
            Event _event = GetEvent(eventId);
            List<string> updatedAttendees = new List<string> ();
            for (int i =0; i <_event.attendees.Count; i++)
            {
                if (_event.attendees[i] != username)
                    updatedAttendees.Add(_event.attendees[i]);
            }

            Event updatedEvent = new Event(_event.ID, _event.priority, _event.plannerUsername,
            _event.eventName, _event.startTime, _event.endTime, _event.teamEvent, updatedAttendees);
           
            _eventStorage.UpdateEvent(updatedEvent);*/


            ////////////////////////////////////////////
            //Decrement the number of event attendees in events table
            //Remove the row username,eventID from isAttendee table
        }


        /// <summary>
        /// Modify an event in the events table
        /// </summary>
        /// <returns></returns>
        public void UpdateEvent(Event updatedEvent)
		{
            // we eant to check for conflict get start and end time of odl event and
            // return a boolean indicating wether there was a time change

            //update event in events table
            _eventsStorage.UpdateEvent(updatedEvent);
		}

        public List<Event> GetEvents(List<int> eventsIDs)
		{
            //if (_eventStorage == null) { return new List<Event>() { }; }
            //EventsStorage _eventsStorage = new EventsStorage();
            return _eventsStorage.GetEvents(eventsIDs);
		}

        private void AddEventToUniversalEventsDB(Event _event)
        {
            _eventsStorage.AddEvent(_event);
        }

        private void RemoveEventFromUniversalEventsDB(int eventId)
        {
            _eventsStorage.RemoveEvent(eventId);
        }

        private void UpdateEventInUniversalEventsDB(Event _event)
        {
            _eventsStorage.UpdateEvent(_event);
        }
    }
}
