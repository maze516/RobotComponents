﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;
using RobotComponents.Parameters;
using RobotComponents.Utils;

namespace RobotComponents.Components
{

    public class InfoComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public InfoComponent()
          : base("Info", "I",
              "Robot Components is a Plugin for intuitive Robot Programming for ABB robots inside Rhinos Grasshopper. The plugin is a development from the Department of Experimental and Digital Design and Construction of the University of Kassel. Supervised by the head of the department Prof. Eversmann. The technical development is executed by student assistant Gabriel Rumpf and research associates Benedikt Wannemacher, Arjen Deetman, Mohamed Dawod, Zuardin Akbar and Andrea Rossi." + Environment.NewLine + Environment.NewLine + "Documentation and Example Files can be found under:" + Environment.NewLine  + Environment.NewLine + "https://github.com/EDEK-UniKassel/RobotComponents/wiki"
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
       
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
        
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponen;
                return Properties.Resources.Info_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4FEC796B-E6F3-4996-84FD-FB6E85FDA16B"); }
        }
    }

}
