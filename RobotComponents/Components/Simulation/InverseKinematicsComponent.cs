﻿using Grasshopper.Kernel;
using Rhino.Geometry;

using System;

using RobotComponents.BaseClasses;
using RobotComponents.Goos;


namespace RobotComponents.Components
{

    public class InverseKinematicsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public InverseKinematicsComponent()
          : base("Inverse Kinematics", "IK",
              "Computes the axis values for a defined ABB robot based on an Action: Target."
                + System.Environment.NewLine +
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Info", "RI", "Robot Info as Robot Info", GH_ParamAccess.item);
            pManager.AddGenericParameter("Target", "T", "Target as Target", GH_ParamAccess.item);
         
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Internal Axis Values", "IAV", "Internal Axis Values");
            pManager.Register_DoubleParam("External Axis Values", "EAV", "External Axis Values");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //variables
            RobotInfoGoo robotInfo = null;
            TargetGoo target= null;

            //inputs
            if (!DA.GetData(0, ref robotInfo)) { return; }
            if (!DA.GetData(1, ref target)) { return; }

            //calculations
            InverseKinematics inverseKinematics = new InverseKinematics(target.Value, robotInfo.Value);
            inverseKinematics.Calculate();

            //output
            DA.SetDataList(0, inverseKinematics.InternalAxisValues);
            DA.SetDataList(1, inverseKinematics.ExternalAxisValues);
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
                //return Resources.IconForThisComponent;
                return Properties.Resources.InverseKinematics_Icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0F1746B8-4E3D-4A22-8719-F7B42C2313AA"); }
        }
    }

}