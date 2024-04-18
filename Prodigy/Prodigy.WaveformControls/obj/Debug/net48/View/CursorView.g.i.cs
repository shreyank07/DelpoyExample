﻿#pragma checksum "..\..\..\..\View\CursorView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B62AD0311D674AD29DEDDA114C8BD0E907B99FD7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Prodigy.WaveformControls.View;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace Prodigy.WaveformControls.View {
    
    
    /// <summary>
    /// CursorView
    /// </summary>
    public partial class CursorView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\View\CursorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Prodigy.WaveformControls.View.CursorView CustomCursor;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\View\CursorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\View\CursorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path path;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\View\CursorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lbl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Prodigy.WaveformControls;component/view/cursorview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\View\CursorView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CustomCursor = ((Prodigy.WaveformControls.View.CursorView)(target));
            
            #line 8 "..\..\..\..\View\CursorView.xaml"
            this.CustomCursor.SizeChanged += new System.Windows.SizeChangedEventHandler(this.CustomCursor_SizeChanged);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\View\CursorView.xaml"
            this.CustomCursor.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.CustomCursor_IsVisibleChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.path = ((System.Windows.Shapes.Path)(target));
            
            #line 12 "..\..\..\..\View\CursorView.xaml"
            this.path.MouseMove += new System.Windows.Input.MouseEventHandler(this.Path_MouseMove);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\..\View\CursorView.xaml"
            this.path.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Path_MouseEnter);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\..\View\CursorView.xaml"
            this.path.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Path_MouseLeave);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\View\CursorView.xaml"
            this.path.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Path_MouseDown);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\View\CursorView.xaml"
            this.path.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Path_MouseUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lbl = ((System.Windows.Controls.TextBlock)(target));
            
            #line 16 "..\..\..\..\View\CursorView.xaml"
            this.lbl.MouseMove += new System.Windows.Input.MouseEventHandler(this.Path_MouseMove);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\..\View\CursorView.xaml"
            this.lbl.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Path_MouseEnter);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\..\View\CursorView.xaml"
            this.lbl.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Path_MouseLeave);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\..\View\CursorView.xaml"
            this.lbl.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Path_MouseDown);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\..\View\CursorView.xaml"
            this.lbl.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Path_MouseUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
