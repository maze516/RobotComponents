﻿using System.IO;
using System.Collections.Generic;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// RAPID Generator class, creates RAPID Code from Actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private RobotInfo _robotInfo; // Robot info to construct the code for
        private List<Action> _actions = new List<Action>(); // List with all the robot actions
        private string _filePath; // File path to save the code
        private bool _saveToFile; // Bool that indicates if the files should be saved
        private string _RAPIDCode; // The rapid main code
        private string _BASECode; // The rapid base code
        private string _ModuleName; // The module name of the rapid main code
        private bool _firstMovementIsMoveAbs; // Bool that indicates if the first movememtn is an absolute joint movement
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty RAPID generator
        /// </summary>
        public RAPIDGenerator()
        {
        }

        /// <summary>
        /// Initiates an RAPID generator. This constructor does not call the methods that create and write the code. 
        /// </summary>
        /// <param name="moduleName"> The name of module / program. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        /// <param name="robotInfo"> The robot info wherefore the code should be created. </param>
        public RAPIDGenerator(string moduleName, List<Action> actions, string filePath, bool saveToFile, RobotInfo robotInfo)
        {
            _ModuleName = moduleName;
            _robotInfo = robotInfo;
            _actions = actions;
            _filePath = filePath;
            _saveToFile = saveToFile;
        }

        /// <summary>
        /// Private constructor with as arguments all the fields of this class. 
        /// This constructor is only used to create duplicates. 
        /// </summary>
        /// <param name="moduleName"> The name of module / program. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        /// <param name="robotInfo"> The robot info wherefore the code should be created. </param>
        /// <param name="rapidCode"> The RAPID main code. </param>
        /// <param name="baseCode"> The BASE code. </param>
        /// <param name="firstMovementIsMoveAbs"> A boolean that indicates if the first movememtn is an absolute joint movement. </param>
        private RAPIDGenerator(string moduleName, List<Action> actions, string filePath, bool saveToFile, RobotInfo robotInfo,
            string rapidCode, string baseCode, bool firstMovementIsMoveAbs)
        {
            _ModuleName = moduleName;
            _robotInfo = robotInfo;
            _actions = actions;
            _filePath = filePath;
            _saveToFile = saveToFile;
            _RAPIDCode = rapidCode;
            _BASECode = baseCode;
            _firstMovementIsMoveAbs = firstMovementIsMoveAbs;
    }
        /// <summary>
        /// Method to duplicate this RAPID generator object.
        /// </summary>
        /// <returns>Returns a deep copy of the RAPID generator object. </returns>
        public RAPIDGenerator Duplicate()
        {
            RAPIDGenerator dup = new RAPIDGenerator(ModuleName, Actions, FilePath, SaveToFile, RobotInfo, RAPIDCode, BASECode, FirstMovementIsMoveAbs);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// Creates the RAPID main codes.
        /// This method also overwrites or creates a file if saved to file is set eqaul to true.
        /// </summary>
        /// <returns> Returns the RAPID main code as a string. </returns>
        public string CreateRAPIDCode()
        {
            // Creates Main Module
            string RAPIDCode = "MODULE " + _ModuleName + "@";

            // Creates Tool Name
            string toolName = _robotInfo.Tool.Name;

            // Creates Vars
            for (int i = 0; i != _actions.Count; i++)
            {

                string tempCode = _actions[i].InitRAPIDVar(_robotInfo, RAPIDCode);

                // Checks if Var is already in Code
                RAPIDCode += tempCode;
            }

            // Create Program
            RAPIDCode += "@@" + "\t" + "PROC main()";

            _firstMovementIsMoveAbs = false;
            bool foundFirstMovement = false;

            // Creates Movement Instruction and other Functions
            for (int i = 0; i != _actions.Count; i++)
            {
                string rapidStr = _actions[i].ToRAPIDFunction(toolName);

                // Checks if first movement is MoveAbsJ
                if (foundFirstMovement == false)
                {
                    if (_actions[i] is Movement)
                    {
                        if (((Movement)_actions[i]).MovementType == 0)
                        {
                            _firstMovementIsMoveAbs = true;
                        }

                        foundFirstMovement = true;
                    }
                }

                // Checks if action is of Type OverrideRobotTool
                if (_actions[i] is OverrideRobotTool)
                {
                    toolName = ((OverrideRobotTool)_actions[i]).GetToolName();
                }

                RAPIDCode += rapidStr;
            }

            // Closes Program
            RAPIDCode += "@" + "\t" + "ENDPROC";
            // Closes Module
            RAPIDCode += "@@" + "ENDMODULE";

            // Replaces@ with newLines
            RAPIDCode = RAPIDCode.Replace("@", System.Environment.NewLine);

            // Update field
            _RAPIDCode = RAPIDCode;

            // Write to file
            if (_saveToFile == true)
            {
                WriteRAPIDCodeToFile();
            }

            // Return
            return RAPIDCode;
        }

        /// <summary>
        /// Creates the RAPID base code with as default tool0, wobj0 and load0. 
        /// This method also overwrites or creates a file if saved to file is set equal to true.
        /// </summary>
        /// <param name="robotTools"> The robot tools that should be added to the BASE code as a list. </param>
        /// <param name="workObjects"> The work objects that should be added to the BASE code as a list. </param>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> Returns the RAPID base code as a string. </returns>
        public string CreateBaseCode(List<RobotTool> robotTools, List<WorkObject> workObjects, List<string> customCode)
        {
            // Creates Main Module
            string BASECode = "MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)@@";

            // Creates Comments
            BASECode += " ! System module with basic predefined system data@";
            BASECode += " !************************************************@@";
            BASECode += " ! System data tool0, wobj0 and load0@";
            BASECode += " ! Do not translate or delete tool0, wobj0, load0@";

            // Creates Predefined System Data
            BASECode += " PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]];@";
            BASECode += " PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0], [1, 0, 0, 0]], [[0, 0, 0], [1, 0, 0, 0]]];@";
            BASECode += " PERS loaddata load0 := [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0];@@";

            // Adds Tools Base Code
            if (robotTools.Count != 0 && robotTools != null)
            {
                BASECode += " ! User defined tooldata @";
                BASECode += CreateToolBaseCode(robotTools);
                BASECode += "@ ";
            }

            // Adds Work Objects Base Code
            if (workObjects.Count != 0 && workObjects != null)
            {
                BASECode += " ! User defined wobjdata @";
                BASECode += CreateWorkObjectBaseCode(workObjects);
                BASECode += "@ ";
            }

            // Adds Custom code line
            if (customCode.Count != 0 && customCode != null)
            {
                BASECode += " ! User definied custom code lines @";
                for (int i = 0; i != customCode.Count; i++)
                {
                    BASECode += customCode[i];
                    BASECode += "@ ";
                }
                BASECode += "@ ";
            }

            // End Module
            BASECode += "ENDMODULE";

            // Replaces @ with newLines
            BASECode = BASECode.Replace("@", System.Environment.NewLine);

            // Update field
            _BASECode = BASECode;

            // Write to file
            if (_saveToFile == true)
            {
                WriteBASECodeToFile();
            }

            // Return
            return BASECode;
        }

        /// <summary>
        /// Gets the Base Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="robotTools"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool base code as a string. </returns>
        private string CreateToolBaseCode(List<RobotTool> robotTools)
        {
            string result = " ";

            for (int i = 0; i != robotTools.Count; i++)
            {
                result += robotTools[i].GetRSToolData();
                result += System.Environment.NewLine + " ";
            }

            return result;
        }

        /// <summary>
        /// Gets the Base Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="workObjects"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool base code as a string. </returns>
        private string CreateWorkObjectBaseCode(List<WorkObject> workObjects)
        {
            string result = " ";

            for (int i = 0; i != workObjects.Count; i++)
            {
                result += workObjects[i].GetWorkObjData();
                result += System.Environment.NewLine + " ";
            }

            return result;
        }

        /// <summary>
        /// Writes the RAPID main code to a file if a file path is set
        /// </summary>
        public void WriteRAPIDCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\main_T.mod", false))
                {
                    writer.WriteLine(_RAPIDCode);
                }
            }
        }

        /// <summary>
        /// Writes the BASE Code to a file if a file path is set
        /// </summary>
        public void WriteBASECodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\BASE.sys", false))
                {
                    writer.WriteLine(_BASECode);
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the RAPID Generator object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Actions == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The Robot Actions as a list. 
        /// </summary>
        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        /// <summary>
        /// The file path where the code will be saved
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// A boolean that indicates if the code files should be saved. 
        /// </summary>
        public bool SaveToFile
        {
            get { return _saveToFile; }
            set { _saveToFile = value; }
        }

        /// <summary>
        /// The main RAPID code
        /// </summary>
        public string RAPIDCode
        {
            get { return _RAPIDCode; }
        }

        /// <summary>
        /// The RAPID Base code
        /// </summary>
        public string BASECode
        {
            get { return _BASECode; }
        }

        /// <summary>
        /// The robot info that is should be uses to create the code for.
        /// </summary>
        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        /// <summary>
        /// The module name of the RAPID main code
        /// </summary>
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }

        /// <summary>
        /// A boolean that indicates if for the first movement an abosulute joint movement is used. 
        /// It is recommended to use for the first movement an absolute joint movement. 
        /// </summary>
        public bool FirstMovementIsMoveAbs
        {
            get { return _firstMovementIsMoveAbs; }
        }
        #endregion
    }

}
