﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraWZycie.Services
{
    public interface IUserMessageService
    {
        void ShowMessage(string message);
    }

    internal class UserMessageService : IUserMessageService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
