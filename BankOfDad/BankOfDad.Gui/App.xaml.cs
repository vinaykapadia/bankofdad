﻿using BankOfDad.Models;

namespace BankOfDad.Gui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}