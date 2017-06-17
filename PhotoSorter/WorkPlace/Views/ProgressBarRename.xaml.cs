﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for ProgressBarRename.xaml
    /// </summary>
    public partial class ProgressBarRename : Window
    {
        public ProgressBarRename(int tempShootId, string tempShootPath, string tempSelectedGrid, System.Collections.ArrayList arrSelectedStudent)
        {
            InitializeComponent();
            this.DataContext = new ProgressBarRenameViewModel(tempShootId, tempShootPath, tempSelectedGrid, arrSelectedStudent);
        }
    }
}
