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

namespace PhotoForce.Mask_Management.Views
{
    /// <summary>
    /// Interaction logic for BulkRenameMaks.xaml
    /// </summary>
    public partial class BulkRenameMaks : Window
    {
        public BulkRenameMaks(System.Collections.ArrayList tempArrMaskId)
        {
            InitializeComponent();
            this.DataContext = new BulkRenameMaksViewModel(tempArrMaskId);
        }
    }
}
