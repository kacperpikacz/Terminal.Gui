﻿using System;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using System.Linq;

namespace UICatalog.Scenarios {
	[ScenarioMetadata (Name: "Tile View Experiments", Description: "Experiments with Tile View")]
	[ScenarioCategory ("Controls")]
	[ScenarioCategory ("LineView")]
	public class TileViewExperiment : Scenario {


		public override void Init (ColorScheme colorScheme)
		{
			Application.Init ();
		}

		/// <summary>
		/// Setup the scenario.
		/// </summary>
		public override void Setup ()
		{
			var menu = new MenuBar (new MenuBarItem [] {
			new MenuBarItem ("_File", new MenuItem [] {
				new MenuItem ("_Quit", "", () => Application.RequestStop()),
			}) });

			Application.Top.Add (menu);

			var frame1 = new FrameView () {
				X = 0,
				Y = 1,
				Width = 15, //Dim.Fill (),
				Height = 15, //Dim.Fill (),
				//IgnoreBorderPropertyOnRedraw = true

			};
			frame1.Border.BorderStyle = BorderStyle.Double;

			var frame2 = new FrameView () {
				X = 0,
				Y = Pos.Bottom (frame1) + 1,
				Width = 15, //Dim.Fill (),
				Height = 15, //Dim.Fill (),
					     //IgnoreBorderPropertyOnRedraw = true

			};
			frame2.Border.BorderStyle = BorderStyle.Single;

			//ConsoleDriver.Diagnostics ^= ConsoleDriver.DiagnosticFlags.FrameRuler;

			Application.Top.Add (frame1);
			Application.Top.Add (frame2);

			var view1 = new TextField () {
				//Title = "View 1",
				Text = "View1 30%/50% Single",
				X = 0,
				Y = 0,
				Width = 14, //Dim.Percent (30) - 5,
				Height = 14, //Dim.Percent (50) - 5,
				ColorScheme = Colors.ColorSchemes ["Dialog"],
				Border = new Border () { 
					BorderStyle = BorderStyle.Single, 
					//BorderThickness = new Thickness (1), 
					DrawMarginFrame = true,
					Padding = new Thickness(1),
					BorderBrush = Color.BrightMagenta,
					Title = "Border Title"
				}
			};

			frame1.Add (view1);

			//var view12splitter = new SplitterEventArgs

			//var view2 = new FrameView () {
			//	Title = "View 2",
			//	Text = "View2 right of view1, 30%/70% Single.",
			//	X = Pos.Right (view1) - 1,
			//	Y = -1,
			//	Width = Dim.Percent (30),
			//	Height = Dim.Percent (70),
			//	ColorScheme = Colors.ColorSchemes ["Error"],
			//	Border = new Border () { BorderStyle = BorderStyle.Single }
			//};

			//frame.Add (view2);

			//var view3 = new FrameView () {
			//	Title = "View 3",
			//	Text = "View3 right of View2 Fill/Fill Single",
			//	X = Pos.Right (view2) - 1,
			//	Y = -1,
			//	Width = Dim.Fill (-1),
			//	Height = Dim.Fill (-1),
			//	ColorScheme = Colors.ColorSchemes ["Menu"],
			//	Border = new Border () { BorderStyle = BorderStyle.Single }
			//};

			//frame.Add (view3);

			//var view4 = new FrameView () {
			//	Title = "View 4",
			//	Text = "View4 below View2 view2.Width/5 Single",
			//	X = Pos.Left (view2),
			//	Y = Pos.Bottom (view2) - 1,
			//	Width = view2.Width,
			//	Height = 5,
			//	ColorScheme = Colors.ColorSchemes ["TopLevel"],
			//	Border = new Border () { BorderStyle = BorderStyle.Single }
			//};

			//frame.Add (view4);

			//var view5 = new FrameView () {
			//	Title = "View 5",
			//	Text = "View5 below View4 view4.Width/5 Double",
			//	X = Pos.Left (view2),
			//	Y = Pos.Bottom (view4) - 1,
			//	Width = view4.Width,
			//	Height = 5,
			//	ColorScheme = Colors.ColorSchemes ["TopLevel"],
			//	Border = new Border () { BorderStyle = BorderStyle.Double }
			//};

			//frame.Add (view5);
		}
	}
}