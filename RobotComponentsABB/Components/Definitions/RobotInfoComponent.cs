﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Definitions;
using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;

namespace RobotComponentsABB.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotInfoComponent : GH_Component
    {
        public RobotInfoComponent()
          : base("Robot Info", "RobInfo",
              "Defines a robot which is needed for Code Generation and Simulation"
                + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item, "Empty RobotInfo");
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Frame", GH_ParamAccess.item);
            pManager.AddParameter(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddParameter(new ExternalAxisParameter(), "External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotInfoParameter(), "Robot Info", "RI", "Resulting Robot Info", GH_ParamAccess.item);  //Todo: beef this up to be more informative.
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            string name = "default robot Info";
            List<Mesh> meshes = new List<Mesh>();
            List<Plane> axisPlanes = new List<Plane>();
            List<Interval> axisLimits = new List<Interval>();
            Plane positionPlane = Plane.WorldXY;
            Plane mountingFrame = Plane.Unset;
            RobotToolGoo toolGoo = null;
            List<ExternalAxis> externalAxis = new List<ExternalAxis>();

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { return; }
            if (!DA.GetDataList(2, axisPlanes))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Axis Points !!!!");
                return;
            }
            if (!DA.GetDataList(3, axisLimits)) { return; }
            if (!DA.GetData(4, ref positionPlane)) { return; }
            if (!DA.GetData(5, ref mountingFrame)) { return; }
            if (!DA.GetData(6, ref toolGoo)) { toolGoo = new RobotToolGoo(); }
            if (!DA.GetDataList(7, externalAxis))
            {
            }

            // Check the axis input: A maximum of one external linear axis is allow
            double count = 0;
            for (int i = 0; i < externalAxis.Count; i++)
            {
                if (externalAxis[i] is ExternalLinearAxis)
                {
                    count += 1;
                }
            }

            // Raise error if more than one external linear axis is used
            if (count > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "At the moment RobotComponents supports one external linear axis.");
            }

            // External axis limits
            for (int i = 0; i < externalAxis.Count; i++)
            {
                axisLimits.Add(externalAxis[i].AxisLimits);
            }

            RobotInfo robotInfo;

            // Override position plane when an external axis is coupled
            if (externalAxis.Count != 0)
            {
                for (int i = 0; i < externalAxis.Count; i++)
                {
                    if (externalAxis[i] is ExternalLinearAxis)
                    {
                        positionPlane = (externalAxis[i] as ExternalLinearAxis).AttachmentPlane;
                    }
                }
                robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value, externalAxis);
            }
            else
            {
                robotInfo = new RobotInfo(name, meshes, axisPlanes, axisLimits, positionPlane, mountingFrame, toolGoo.Value);
            }

            // Output
            DA.SetData(0, robotInfo);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobotInfo_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D62D3E73-6D93-4E80-9892-591DBEA648BE"); }
        }

        public override string ToString()
        {
            return "Robot Info";
        }
    }
}
