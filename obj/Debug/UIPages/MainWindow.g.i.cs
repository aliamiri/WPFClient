﻿#pragma checksum "..\..\..\UIPages\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D576238C5C635F885864D048EA0C1CF3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WpfNotifierClient.UIPages {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem LoginReportItem;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem CreateUserItem;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem AutoCheckItem;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ChangeThemeItem;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DgTrxInfo;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\UIPages\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ReconnectButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AsanPardakhtNotifier;component/uipages/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UIPages\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 13 "..\..\..\UIPages\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_IntervalReport);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LoginReportItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 14 "..\..\..\UIPages\MainWindow.xaml"
            this.LoginReportItem.Click += new System.Windows.RoutedEventHandler(this.MenuItem_LoginReport);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 17 "..\..\..\UIPages\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_lock);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 18 "..\..\..\UIPages\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_signOut);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CreateUserItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 19 "..\..\..\UIPages\MainWindow.xaml"
            this.CreateUserItem.Click += new System.Windows.RoutedEventHandler(this.MenuItem_createNewUser);
            
            #line default
            #line hidden
            return;
            case 6:
            this.AutoCheckItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 23 "..\..\..\UIPages\MainWindow.xaml"
            this.AutoCheckItem.Click += new System.Windows.RoutedEventHandler(this.MenuItem_AutomaticCheck);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ChangeThemeItem = ((System.Windows.Controls.MenuItem)(target));
            
            #line 24 "..\..\..\UIPages\MainWindow.xaml"
            this.ChangeThemeItem.Click += new System.Windows.RoutedEventHandler(this.MenuItem_AutomaticCheck);
            
            #line default
            #line hidden
            return;
            case 8:
            this.DgTrxInfo = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.ReconnectButton = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\UIPages\MainWindow.xaml"
            this.ReconnectButton.Click += new System.Windows.RoutedEventHandler(this.Reconnect_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

