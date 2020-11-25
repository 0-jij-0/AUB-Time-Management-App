﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUBTimeManagementApp.GUI
{
    public partial class AddEvent : Form
    {
        mainForm parent;
        public AddEvent(mainForm _parent)
        {
            parent = _parent;
            InitializeComponent();

            startDate.Value = DateTime.Now;
            endDate.Value = DateTime.Now.AddMinutes(30);
        }

        public AddEvent(mainForm _parent, DateTime _startDate, DateTime _endDate) : this(_parent) {
            startDate.Value = _startDate;
            endDate.Value = _endDate;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Client.Client.Instance.showEvent(0,eventName.Text, priority.Value, startDate.Value, endDate.Value);
            Client.Client.Instance.CreatePersonalEvent(eventName.Text, priority.Value, startDate.Value, endDate.Value);
            Close();
        }
    }
}
