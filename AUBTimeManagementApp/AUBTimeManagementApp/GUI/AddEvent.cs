﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AUBTimeManagementApp.Service;

namespace AUBTimeManagementApp.GUI
{
    public partial class AddEvent : Form
    {
        Form1 parent;
        public AddEvent(Form1 _parent)
        {
            parent = _parent;
            Client.Client.Instance.setForm(this);

            InitializeComponent();
        }

        public AddEvent(Form1 _parent, DateTime _startDate, DateTime _endDate) : this(_parent) {
            startDate.Value = _startDate;
            endDate.Value = _endDate;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Client.Client.Instance.createEvent(eventName.Text, priority.Value, startDate.Value, endDate.Value);
            parent.displayEvent(eventName.Text, priority.Value, startDate.Value, endDate.Value);
            Close();
        }

        private void AddEvent_Load(object sender, EventArgs e)
        {

        }
    }
}
