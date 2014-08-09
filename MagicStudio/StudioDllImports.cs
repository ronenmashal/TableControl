using System;
using System.Runtime.InteropServices;
using System.Text;
using com.magicsoftware.util;
using MagicSoftware.Studio.Bridge;

namespace MagicStudio
{
   public class StudioDllImports : StudioDllWrapper
   {
      public const String StudioDll = @"d:\dev-magic\_Trunk\Sources\MgxpaStudioCore.dll";

      private IValidation validationImpl;

      public StudioDllImports()
      {
         validationImpl = new ValidationImpl();
      }

      public override StudioDllWrapper.IValidation Validation { get { return validationImpl; } }

      public override void FocuseOnOldStudio()
      {
         Imports.FocuseOnOldStudio();
      }

      public override void PutActionToQueue(int action)
      {
         Imports.PutActionToQueue(action);
      }

      public override void RestoreStateBeforeOpenFormDesigner(bool dropState)
      {
         Imports.RestoreStateBeforeOpenFormDesigner(dropState);
      }
      public override void getUnLimitedString(ref IntPtr str)
      {
         Imports.getUnLimitedString(ref str);
      }

      public override void getLimitedString(StringBuilder str)
      {
         Imports.getLimitedString(str);
      }

      public override void getInt(ref short str)
      {
         Imports.getInt(ref str);
      }

      public override string getUnicodeString()
      {
         return Imports.getUnicodeString();
      }

      public override void TestMessageInfo(out IntPtr Result)
      {
         Imports.TestMessageInfo(out Result);
      }

      public override bool StudioDllInit(string lpszCmdParam, out IntPtr iDotNetService)
      {
         return Imports.StudioDllInit(lpszCmdParam, out iDotNetService);
      }

      public override bool StudioDllInitThread()
      {
         return Imports.StudioDllInitThread();
      }

      public override void StudioDllClean()
      {
         Imports.StudioDllClean();
      }

      public override bool CreateNewProject(string EdpFileName)
      {
         return Imports.CreateNewProject(EdpFileName);
      }

      public override IntPtr OpenProjectPhase1(string EdpFileName)
      {
         return Imports.OpenProjectPhase1(EdpFileName);
      }

      public override IntPtr OpenProjectPhase2(string EdpFileName, IntPtr messageInfo)
      {
         return Imports.OpenProjectPhase2(EdpFileName, messageInfo);
      }

      public override void CloseProject()
      {
         Imports.CloseProject();
      }

      public override IntPtr CheckVersionAndComponentLoaded(out bool isDefaultComponentLoaded)
      {
         return Imports.CheckVersionAndComponentLoaded(out isDefaultComponentLoaded);
      }

      public override void StartMigration(bool shouldAddUserFuncComponent)
      {
         Imports.StartMigration(shouldAddUserFuncComponent);
      }

      public override bool Open(int ObjectHandle, int ObjectType)
      {
         return Imports.Open(ObjectHandle, ObjectType);
      }

      public override bool OpenSecondary(int SecondaryObjectHandle, int PrimaryType, int SecondaryType)
      {
         return Imports.OpenSecondary(SecondaryObjectHandle, PrimaryType, SecondaryType);
      }

      public override bool OpenObjectInCollection(int hdl, int PrimaryType, int IsnObject)
      {
         return Imports.OpenObjectInCollection(hdl, PrimaryType, IsnObject);
      }

      public override bool OpenAndLoadObjectInCollection(int PrimaryObjectHandle, int PrimaryType, int PrimaryIsn, out IntPtr Result)
      {
         return Imports.OpenAndLoadObjectInCollection(PrimaryObjectHandle, PrimaryType, PrimaryIsn, out Result);
      }

      public override bool CloseAndUnLoadObjectInCollection(int ObjectHandle)
      {
         return Imports.CloseAndUnLoadObjectInCollection(ObjectHandle);
      }

      public override void SetParentHandle(int childHandle, int newParentHandle)
      {
         Imports.SetParentHandle(childHandle, newParentHandle);
      }

      public override bool UnLoadObjectInCollection(int ObjectHandle)
      {
         return Imports.UnLoadObjectInCollection(ObjectHandle);
      }

      public override bool LoadObjectInCollection(int ObjectHandle, out IntPtr Result)
      {
         return Imports.LoadObjectInCollection(ObjectHandle, out Result);
      }

      public override bool Close(int ObjectHandle)
      {
         return Imports.Close(ObjectHandle);
      }

      public override bool Save(int ObjectHandle)
      {
         return Imports.Save(ObjectHandle);
      }

      public override void StateSave(int ObjectHandle, bool allTasks)
      {
         Imports.StateSave(ObjectHandle, allTasks);
      }

      public override void StateRestore(int ObjectHandle)
      {
         Imports.StateRestore(ObjectHandle);
      }

      public override void StateDrop(int ObjectHandle)
      {
         Imports.StateDrop(ObjectHandle);
      }

      public override bool StateIsModified(int ObjectHandle)
      {
         return Imports.StateIsModified(ObjectHandle);
      }

      public override void GenerateProgram(StudioDllWrapper.APG_ITEM apg)
      {
         Imports.GenerateProgram(apg);
      }

      public override void StopProgram()
      {
         Imports.StopProgram();
      }

      public override void RunProgram(int isn)
      {
         Imports.RunProgram(isn);
      }

      public override int CreateControl(int ObjectHandle, int controlType, StudioDllWrapper.ControlSubType controlSubType, int afterIsn, int parentIsn, int x, int y, byte layer)
      {
         return Imports.CreateControl(ObjectHandle, controlType, controlSubType, afterIsn, parentIsn, x, y, layer);
      }

      public override int DeleteControl(int ObjectHandle, int controlIsn)
      {
         return Imports.DeleteControl(ObjectHandle, controlIsn);
      }

      public override int DeleteControls(int objectHandle, IntPtr controlIsns, int controlCount)
      {
         return Imports.DeleteControls(objectHandle, controlIsns, controlCount);
      }

      public override int AddColumn(int ObjectHandle, int layer, IntPtr name)
      {
         return Imports.AddColumn(ObjectHandle, layer, name);
      }

      public override void AttachControls(int ObjectHandle, int parentIsn, IntPtr childIsns, int childCount, int layer)
      {
         Imports.AttachControls(ObjectHandle, parentIsn, childIsns, childCount, layer);
      }

      public override void AddControlsInContainer(int ObjectHandle, int parentIsn, int layer, IntPtr childIsns, IntPtr locations, int count)
      {
         Imports.AddControlsInContainer(ObjectHandle, parentIsn, layer, childIsns, locations, count);
      }


      public override void DetachControls(int ObjectHandle, IntPtr childIsns, int childCount)
      {
         Imports.DetachControls(ObjectHandle, childIsns, childCount);
      }

      public override void ChangeControlZOrder(int ObjectHandle, int childIsn, int newOrder)
      {
         Imports.ChangeControlZOrder(ObjectHandle, childIsn, newOrder);
      }

      public override void FitControlsSize(int ObjectHandle, IntPtr controls, int count)
      {
         Imports.FitControlsSize(ObjectHandle, controls, count);
      }

      public override IntPtr CopyControls(int ObjectHandle, IntPtr controls, int count, out IntPtr Result)
      {
         return Imports.CopyControls(ObjectHandle, controls, count, out Result);
      }

      public override bool PasteControls(int ObjectHandle, IntPtr dataHandle, int xy, out IntPtr Result)
      {
         return Imports.PasteControls(ObjectHandle, dataHandle, xy, out Result);
      }

      public override void SaveUndoData(int ObjectHandle, IntPtr controls, int count)
      {
         Imports.SaveUndoData(ObjectHandle, controls, count);
      }

      public override int DoUndoRedo(int ObjectHandle, UndoRedoAction redo, out IntPtr markedContolsIsns)
      {
         return Imports.DoUndoRedo(ObjectHandle, redo, out markedContolsIsns);
      }

      public override int CreateNewControlForVariable(int ObjectHandle, int x, int y, IntPtr referencedKeyIntPtr, int parentIsn, int modelType, ControlSubType controlSubType, bool isShiftKeyPressed, byte layer, out IntPtr controlIsnArrayPtr)
      {
         return Imports.CreateNewControlForVariable(ObjectHandle, x, y, referencedKeyIntPtr, parentIsn, modelType, controlSubType, isShiftKeyPressed, layer, out controlIsnArrayPtr);
      }

      public override int GetViewFieldIndex(int objHandle, int nodeIsn)
      {
         return Imports.GetViewFieldIndex(objHandle, nodeIsn);
      }

      public override void ExecuteIncludeInView(int objHandle, int nodeIsn)
      {
         Imports.ExecuteIncludeInView(objHandle, nodeIsn);
      }

      public override void ExecuteExcludeFromView(int objHandle, int nodeIsn)
      {
         Imports.ExecuteExcludeFromView(objHandle, nodeIsn);
      }

      public override void DbhInit()
      {
         Imports.DbhInit();
      }

      public override void DbhCleanUp()
      {
         Imports.DbhCleanUp();
      }

      public override void OnEnterDbColumns(int objHandle)
      {
         Imports.OnEnterDbColumns(objHandle);
      }

      public override void OnLeaveDbColumns(int objHandle)
      {
         Imports.OnLeaveDbColumns(objHandle);
      }

      public override void OnDbhSelectedIndexChanged(int objectHandle, int DbhIsn)
      {
         Imports.OnDbhSelectedIndexChanged(objectHandle, DbhIsn);
      }

      public override void OnDbhSelectionChanged(int objectHandle, int DbhIsn)
      {
         Imports.OnDbhSelectionChanged(objectHandle, DbhIsn);
      }

      public override void OnEnterDbIndexes(int objHandle)
      {
         Imports.OnEnterDbIndexes(objHandle);
      }

      public override void OnLeaveDbIndexes(int objHandle)
      {
         Imports.OnLeaveDbIndexes(objHandle);
      }

      public override void OnEnterDbForeignKeys(int objHandle)
      {
         Imports.OnEnterDbForeignKeys(objHandle);
      }

      public override void OnLeaveDbForeignKeys(int objHandle)
      {
         Imports.OnLeaveDbForeignKeys(objHandle);
      }

      public override void DbhColumnsStorageCheck(int objectHandle, int objectIsn, int fldStorage, int fldLength, out IntPtr messageInfo)
      {
         Imports.DbhColumnsStorageCheck(objectHandle, objectIsn, fldStorage, fldLength, out messageInfo);
      }

      public override void OnDbhFldModelChangeSynchronizePropertiesToFld(int objectHandle, int objectIsn)
      {
         Imports.OnDbhFldModelChangeSynchronizePropertiesToFld(objectHandle, objectIsn);
      }

      public override void SynchronizePropertiesToFld(int objectHandle, int objectIsn, int propType)
      {
         Imports.SynchronizePropertiesToFld(objectHandle, objectIsn, propType);
      }

      public override bool Reload(int ObjectHandle, bool partial, out IntPtr Result)
      {
         return Imports.Reload(ObjectHandle, partial, out Result);
      }

      public override int GetCount(int ObjectHandle)
      {
         return Imports.GetCount(ObjectHandle);
      }


      public override void ReInitSpecificDataInDll(int ObjectHandle)
      {
         Imports.ReInitSpecificDataInDll(ObjectHandle);
      }

      public override int GetKeys(int ObjectHandle, out IntPtr keys)
      {
         return Imports.GetKeys(ObjectHandle, out keys);
      }

      public override bool GetLine(int ObjectHandle, int isn, out IntPtr line)
      {
         return Imports.GetLine(ObjectHandle, isn, out line);
      }

      public override bool GetData(int ObjectHandle, out IntPtr line)
      {
         return Imports.GetData(ObjectHandle, out line);
      }

      public override int DeleteLine(int ObjectHandle, int Isn, out IntPtr ReturnResult)
      {
         return Imports.DeleteLine(ObjectHandle, Isn, out ReturnResult);
      }

      public override int CreateLine(int ObjectHandle, int AfterIsn, int inHeaderIsn)
      {
         return Imports.CreateLine(ObjectHandle, AfterIsn, inHeaderIsn);
      }

      public override int GoToObject(int ObjectHandle, int isn)
      {
         return Imports.GoToObject(ObjectHandle, isn);
      }

      public override int CreateSpecialObject(int ObjectHandle, IntPtr extraData)
      {
         return Imports.CreateSpecialObject(ObjectHandle, extraData);
      }

      public override int CreateEventArgument(int breakIsn)
      {
         return Imports.CreateEventArgument(breakIsn);
      }

      public override void HasBeenModified(int ObjectHandle, bool isMod)
      {
         Imports.HasBeenModified(ObjectHandle, isMod);
      }

      public override StudioDllWrapper.ReturnValue UpdateLine(int ObjectHandle, ref IntPtr line, out IntPtr ReturnResult)
      {
         return Imports.UpdateLine(ObjectHandle, ref line, out ReturnResult);
      }

      public override bool OpenChildCollection(int parentHandle, int childHandle, int parentIsn, int childType, out IntPtr returnResult)
      {
         return Imports.OpenChildCollection(parentHandle, childHandle, parentIsn, childType, out returnResult);
      }

      public override void HandlePropertyChanged(int ObjectHandle, ref IntPtr line, ref IntPtr newValue, ref IntPtr oldValue, String propName, out IntPtr returnResult)
      {
         Imports.HandlePropertyChanged(ObjectHandle, ref line, ref newValue, ref oldValue, propName, out returnResult);
      }

      public override void CancelProgressBarOperation(bool cancelOperation)
      {
         Imports.CancelProgressBarOperation(cancelOperation);
      }

      public override void Export(char ExportType, string FileName, int RangeFrom, int RangeTo, bool WithModel, out IntPtr ReturnResult)
      {
         Imports.Export(ExportType, FileName, RangeFrom, RangeTo, WithModel, out ReturnResult);
      }

      public override void Import(string ImportFileName, out IntPtr returnResult)
      {
         Imports.Import(ImportFileName, out returnResult);
      }

      public override void RegisterCallbacks(CallbackType callbackType, int callbackCnt, IntPtr callbackPtrsArray)
      {
         Imports.RegisterCallbacks(callbackType, callbackCnt, callbackPtrsArray);
      }

      public override void UnRegisterCallbacks(CallbackType callbackType)
      {
         Imports.UnRegisterCallbacks(callbackType);
      }

      public override bool TranslateStringWithLogicalNames(string OrgString, StringBuilder ResultString)
      {
         return Imports.TranslateStringWithLogicalNames(OrgString, ResultString);
      }

      public override bool TranslateStringWithLanguage(IntPtr OrgString, out IntPtr ResultString)
      {
         return Imports.TranslateStringWithLanguage(OrgString, out ResultString);
      }

      public override void PictureToPic(char attr, IntPtr picture, IntPtr pic, IntPtr sample)
      {
         Imports.PictureToPic(attr, picture, pic, sample);
      }

      public override void PicToPicture(char attr, IntPtr pic, IntPtr sample)
      {
         Imports.PicToPicture(attr, pic, sample);
      }

      public override bool CreateCabinetFile(string FileName)
      {
         return Imports.CreateCabinetFile(FileName);
      }

      public override void MoveItemToFolder(int hdl, int ItemIsn, int FolderIsn)
      {
         Imports.MoveItemToFolder(hdl, ItemIsn, FolderIsn);
      }

      public override void PerformLogicHeaderAction(int handle, ref IntPtr line, LogicHeaderAction logicHeaderAction)
      {
         Imports.PerformLogicHeaderAction(handle, ref line, logicHeaderAction);
      }

      public override int LineManipulation(int ObjectHandle, int lineManipulationType, IntPtr afterElementIsn, int inHeaderIsn, IntPtr lineManipulationRange, int lineManipulationRangeCount)
      {
         return Imports.LineManipulation(ObjectHandle, lineManipulationType, afterElementIsn, inHeaderIsn, lineManipulationRange, lineManipulationRangeCount);
      }

      public override void PerformDataCollectionAction(StudioDllWrapper.DataCollectionAction action, int objectHandle, ref IntPtr extraData, out IntPtr returnResult)
      {
         Imports.PerformDataCollectionAction(action, objectHandle, ref extraData, out returnResult);
      }

      public override bool ReloadXmlDefinition(int dbhIsn, bool replaceRelatedViews)
      {
         return Imports.ReloadXmlDefinition(dbhIsn, replaceRelatedViews);
      }

      public override bool OpenHelpWindow(int ObjectHandle, int isn, int propId, bool isPickList)
      {
         return Imports.OpenHelpWindow(ObjectHandle, isn, propId, isPickList);
      }


      public override bool OpenTree(int SecondaryObjectHandle, int SecondaryType, int ParentElementType, int ParentElementIsn)
      {
         return Imports.OpenTree(SecondaryObjectHandle, SecondaryType, ParentElementType, ParentElementIsn);
      }

      public override bool CloseTree(int ObjectHandle)
      {
         return Imports.CloseTree(ObjectHandle);
      }

      public override bool InsertNode(int ObjectHandle, int index, bool isRoot)
      {
         return Imports.InsertNode(ObjectHandle, index, isRoot);
      }

      public override bool LeaveNode(int ObjectHandle, bool isRoot)
      {
         return Imports.LeaveNode(ObjectHandle, isRoot);
      }

      public override bool GetNodeData(int ObjectHandle, out IntPtr line)
      {
         return Imports.GetNodeData(ObjectHandle, out line);
      }

      public override bool SetNodeData(int ObjectHandle, IntPtr line, IntPtr newVal)
      {
         return Imports.SetNodeData(ObjectHandle, line, newVal);
      }

      public override int GetChildrenCount(int ObjectHandle)
      {
         return Imports.GetChildrenCount(ObjectHandle);
      }

      public override bool CreateChildNode(int ObjectHandle, IntPtr parentNodeInfo, IntPtr defaultNodeValue, out IntPtr childNodeInfo)
      {
         return Imports.CreateChildNode(ObjectHandle, parentNodeInfo, defaultNodeValue, out childNodeInfo);
      }

      public override bool CreateParentNode(int ObjectHandle, IntPtr childNodeInfo, int childNodeHashCode, IntPtr defaultNodeValue, out IntPtr parentNodeInfo)
      {
         return Imports.CreateParentNode(ObjectHandle, childNodeInfo, childNodeHashCode, defaultNodeValue, out parentNodeInfo);
      }

      public override bool DeleteNode(int ObjectHandle, IntPtr parentNodeInfo, int nodeIndexInParent)
      {
         return Imports.DeleteNode(ObjectHandle, parentNodeInfo, nodeIndexInParent);
      }

      public override bool CopyNode(int ObjectHandle, IntPtr nodeInfo)
      {
         return Imports.CopyNode(ObjectHandle, nodeInfo);
      }

      public override bool CutNode(int ObjectHandle, IntPtr nodeInfo)
      {
         return Imports.CutNode(ObjectHandle, nodeInfo);
      }

      public override bool IsInCutState(int ObjectHandle)
      {
         return Imports.IsInCutState(ObjectHandle);
      }

      public override void ResetIsInCutState(int ObjectHandle)
      {
         Imports.ResetIsInCutState(ObjectHandle);
      }

      public override bool PasteNode(int ObjectHandle, int pasteType, IntPtr nodeInfo, out IntPtr newRootPastedNode)
      {
         return Imports.PasteNode(ObjectHandle, pasteType, nodeInfo, out newRootPastedNode);
      }

      public override bool IsCopyAllowed(int ObjectHandle, IntPtr nodeInfo, out IntPtr returnResult)
      {
         return Imports.IsCopyAllowed(ObjectHandle, nodeInfo, out returnResult);
      }

      public override bool IsCutAllowed(int ObjectHandle, IntPtr nodeInfo, out IntPtr returnResult)
      {
         return Imports.IsCutAllowed(ObjectHandle, nodeInfo, out returnResult);
      }

      public override bool IsPasteAllowed(int ObjectHandle, IntPtr nodeInfo, int pasteType, out IntPtr returnResult)
      {
         return Imports.IsPasteAllowed(ObjectHandle, nodeInfo, pasteType, out returnResult);
      }

      public override bool RefreshNodeData(int ObjectHandle, IntPtr NodeInfo, out IntPtr result)
      {
         return Imports.RefreshNodeData(ObjectHandle, NodeInfo, out result);
      }

      public override void ChangeAutomaticZOrTabOrder(int ObjectHandle)
      {
         Imports.ChangeAutomaticZOrTabOrder(ObjectHandle);
      }

      public override bool IsDotNetTypeAssignable(IntPtr mgAttrItemTo, IntPtr mgAttrItemFrom)
      {
         return Imports.IsDotNetTypeAssignable(mgAttrItemTo, mgAttrItemFrom);
      }

      public override IntPtr GetDotNetMemberType(IntPtr objectType, IntPtr memberName)
      {
         return Imports.GetDotNetMemberType(objectType, memberName);
      }

      public override bool IsDotNetTypeControl(IntPtr typeName)
      {
         return Imports.IsDotNetTypeControl(typeName);
      }

      public override void ParseExpressionText(IntPtr str, out IntPtr result)
      {
         Imports.ParseExpressionText(str, out result);
      }

      public override void SetCurrentTaskAsActive(int ObjectHandle)
      {
         Imports.SetCurrentTaskAsActive(ObjectHandle);
      }

      public override void GetAutoCompletionList(IntPtr str, out IntPtr result)
      {
         Imports.GetAutoCompletionList(str, out result);
      }

      public override IntPtr GetFunctionSignatureText(IntPtr str)
      {
         return Imports.GetFunctionSignatureText(str);
      }

      public override IntPtr GetRtfFromText(IntPtr str)
      {
         return Imports.GetRtfFromText(str);
      }

      class ValidationImpl : StudioDllWrapper.IValidation
      {

         public IntPtr ExpCompatible(int ObjectHandle, int isn, IntPtr mgAttrItem)
         {
            return Imports.ExpCompatible(ObjectHandle, isn, mgAttrItem);
         }

         public char ExpAttr(int ObjectHandle, int expressionIndex, out IntPtr result)
         {
            return Imports.ExpAttr(ObjectHandle, expressionIndex, out result);
         }

         public bool CheckCallProgram(int isn)
         {
            return Imports.CheckCallProgram(isn);
         }

         public void ChkExpRTFResult(int ObjectHandle, int expIdx, int isRTFEdit, int caseAllowed, IntPtr mgAttrItem, out IntPtr result)
         {
            Imports.ChkExpRTFResult(ObjectHandle, expIdx, isRTFEdit, caseAllowed, mgAttrItem, out result);
         }

         public bool ExpMustExecOnClient(int ObjectHandle, int expressionIsn)
         {
            return Imports.ExpMustExecOnClient(ObjectHandle, expressionIsn);
         }

         public void CheckerInitialize(int ObjectHandle)
         {
            Imports.CheckerInitialize(ObjectHandle);
         }

         public void CheckerTerminate(int ObjectHandle)
         {
            Imports.CheckerTerminate(ObjectHandle);
         }

         public int GetUnusedExpressionIndices(int ObjectHandle, out IntPtr unusedExpIndicesPtr)
         {
            return Imports.GetUnusedExpressionIndices(ObjectHandle, out unusedExpIndicesPtr);
         }
      }


      /// <summary>
      /// This is a private class that actually imports the method implementations from 
      /// the relevant DLL.
      /// </summary>
      class Imports
      {
         #region Methods

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void FocuseOnOldStudio();

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void PutActionToQueue(int action);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void RestoreStateBeforeOpenFormDesigner(bool dropState);

         [DllImport(StudioDll, CallingConvention = CallingConvention.StdCall)]
         public static extern void getUnLimitedString(ref IntPtr str);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void getLimitedString([MarshalAs(UnmanagedType.LPStr)] StringBuilder str);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void getInt(ref Int16 str);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         [return: MarshalAs(UnmanagedType.LPWStr)]
         public static extern string getUnicodeString();


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void TestMessageInfo(out IntPtr Result);


         // Int\Term DLL
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool StudioDllInit([MarshalAs(UnmanagedType.LPStr)] String lpszCmdParam, out IntPtr iDotNetService);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool StudioDllInitThread();


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StudioDllClean();


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CreateNewProject([MarshalAs(UnmanagedType.LPStr)] String EdpFileName);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr OpenProjectPhase1([MarshalAs(UnmanagedType.LPStr)] String EdpFileName);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr OpenProjectPhase2([MarshalAs(UnmanagedType.LPStr)] String EdpFileName, IntPtr messageInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void CloseProject();


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr CheckVersionAndComponentLoaded(out bool isDefaultComponentLoaded);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StartMigration(bool shouldAddUserFuncComponent);


         // Handle: I_REPOSITORY_BASE
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool Open(int ObjectHandle, int ObjectType);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool OpenSecondary(int SecondaryObjectHandle, int PrimaryType, int SecondaryType);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool OpenObjectInCollection(int hdl, int PrimaryType, int IsnObject);



         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool OpenAndLoadObjectInCollection(int PrimaryObjectHandle, int PrimaryType, int PrimaryIsn, out IntPtr Result);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CloseAndUnLoadObjectInCollection(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void SetParentHandle(int childHandle, int newParentHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool UnLoadObjectInCollection(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool LoadObjectInCollection(int ObjectHandle, out IntPtr Result);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool Close(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool Save(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StateSave(int ObjectHandle, bool allTasks);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StateRestore(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StateDrop(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool StateIsModified(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void GenerateProgram(APG_ITEM apg);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void StopProgram();


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void RunProgram(int isn);




         #region FormDesigner methods

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int CreateControl(int ObjectHandle, int controlType, ControlSubType controlSubType, int afterIsn, int parentIsn, int x, int y, byte layer);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int DeleteControl(int ObjectHandle, int controlIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int DeleteControls(int objectHandle, IntPtr controlIsns, int controlCount);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int AddColumn(int ObjectHandle, int layer, IntPtr name);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void AttachControls(int objectHandle, int parentIsn, IntPtr childIsns, int childIsn, int layer);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void AddControlsInContainer(int ObjectHandle, int parentIsn, int layer, IntPtr childIsns, IntPtr locations, int count);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void DetachControls(int objectHandle, IntPtr childIsns, int childCount);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ChangeControlZOrder(int ObjectHandle, int childIsn, int newOrder);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void FitControlsSize(int ObjectHandle, IntPtr controls, int count);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr CopyControls(int ObjectHandle, IntPtr controls, int count, out IntPtr Result);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool PasteControls(int ObjectHandle, IntPtr dataHandle, int xy, out IntPtr Result);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void SaveUndoData(int ObjectHandle, IntPtr controls, int count);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int DoUndoRedo(int ObjectHandle, UndoRedoAction action, out IntPtr markedControls);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int CreateNewControlForVariable(int ObjectHandle, int x, int y, IntPtr referencedKeyIntPtr, int parentIsn, int modelType, ControlSubType controlSubType, bool isShiftKeyPressed, byte layer, out IntPtr controlIsnArrayPtr);


         #endregion

         #region MetaTree methods

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GetViewFieldIndex(int objHandle, int nodeIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ExecuteIncludeInView(int objHandle, int nodeIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ExecuteExcludeFromView(int objHandle, int nodeIsn);


         #endregion
         #region DataSource methods

         /// <summary>
         /// Calls init code in dll when Dbh pane is opened
         /// </summary>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void DbhInit();


         /// <summary>
         /// Calls cleanup code in dll when Dbh pane is closed
         /// </summary>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void DbhCleanUp();


         /// <summary>
         /// init code in dll while entering DbColumns TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnEnterDbColumns(int objHandle);


         /// <summary>
         /// cleanup code in dll while entering DbColumns TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnLeaveDbColumns(int objHandle);


         /// <summary>
         /// calls the code in dll when parking is changing to another row in DataRepository.
         /// Includes commiting of Dbh
         /// </summary>
         /// <param name="DbhIsn"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnDbhSelectedIndexChanged(int objectHandle, int DbhIsn);


         /// <summary>
         /// calls the code in dll when parking is changed to another row in 
         /// Data Repository.
         /// </summary>
         /// <param name="DbhIsn"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnDbhSelectionChanged(int objectHandle, int DbhIsn);


         /// <summary>
         /// init code in dll while entering DbIndexes TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnEnterDbIndexes(int objHandle);


         /// <summary>
         /// cleanup code in dll while entering DbIndexes TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnLeaveDbIndexes(int objHandle);


         /// <summary>
         /// init code in dll while entering DbForKey TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnEnterDbForeignKeys(int objHandle);


         /// <summary>
         /// cleanup code in dll while entering DbForKey TabItem
         /// </summary>
         /// <param name="objHandle"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnLeaveDbForeignKeys(int objHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void DbhColumnsStorageCheck(int objectHandle, int objectIsn, int fldStorage, int fldLength, out IntPtr messageInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void OnDbhFldModelChangeSynchronizePropertiesToFld(int objectHandle, int objectIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void SynchronizePropertiesToFld(int objectHandle, int objectIsn, int propType);


         #endregion

         /// <summary>
         /// makes the dll to reload data from the xml to the memory
         /// </summary>
         /// <param name="hdl"></param>
         /// <returns></returns>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool Reload(int ObjectHandle, bool partial, out IntPtr Result);



         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GetCount(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ReInitSpecificDataInDll(int ObjectHandle);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GetKeys(int ObjectHandle, out IntPtr keys);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool GetLine(int ObjectHandle, int isn, out IntPtr line);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool GetData(int ObjectHandle, out IntPtr line);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int DeleteLine(int ObjectHandle, int Isn, out IntPtr ReturnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int CreateLine(int ObjectHandle, int AfterIsn, int inHeaderIsn);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GoToObject(int ObjectHandle, int isn);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int CreateSpecialObject(int ObjectHandle, IntPtr extraData);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int CreateEventArgument(int breakIsn);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void HasBeenModified(int ObjectHandle, bool isMod);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern ReturnValue UpdateLine(int ObjectHandle, ref IntPtr line, out IntPtr ReturnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void HandlePropertyChanged(int ObjectHandle, ref IntPtr line, ref IntPtr newValue, ref IntPtr oldValue,
                                                         [MarshalAs(UnmanagedType.LPStr)] String propName, out IntPtr returnResult);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         [return: MarshalAs(UnmanagedType.I1)]
         public static extern bool OpenChildCollection(int parentHandle, int childHandle, int parentIsn, int childType, out IntPtr returnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void CancelProgressBarOperation(bool cancelOperation);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void Export(char ExportType, [MarshalAs(UnmanagedType.LPStr)] String FileName, int RangeFrom, int RangeTo, bool WithModel, out IntPtr ReturnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void Import(String ImportFileName, out IntPtr returnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void RegisterCallbacks(CallbackType callbackType, int callbackCnt, IntPtr callbackPtrsArray);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void UnRegisterCallbacks(CallbackType callbackType);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool TranslateStringWithLogicalNames([MarshalAs(UnmanagedType.LPStr)] String OrgString, [MarshalAs(UnmanagedType.LPStr)] StringBuilder ResultString);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool TranslateStringWithLanguage(IntPtr OrgString, out IntPtr ResultString);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void PictureToPic(char attr, IntPtr picture, IntPtr pic, IntPtr sample);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void PicToPicture(char attr, IntPtr pic, IntPtr sample);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CreateCabinetFile([MarshalAs(UnmanagedType.LPStr)] String FileName);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void MoveItemToFolder(int hdl, int ItemIsn, int FolderIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void PerformLogicHeaderAction(int handle, ref IntPtr line, LogicHeaderAction logicHeaderAction);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int LineManipulation(int ObjectHandle, int lineManipulationType, IntPtr afterElementIsn, int inHeaderIsn, IntPtr lineManipulationRange, int lineManipulationRangeCount);


         /// <summary>
         /// Perform action on particular object of DataCollection
         /// </summary>
         /// <param name="action"></param>
         /// <param name="objectHandle"></param>
         /// <param name="extraData"></param>
         /// <param name="returnResult"></param>
         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void PerformDataCollectionAction(DataCollectionAction action, int objectHandle, ref IntPtr extraData, out IntPtr returnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool ReloadXmlDefinition(int dbhIsn, bool replaceRelatedViews);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool OpenHelpWindow(int ObjectHandle, int isn, int propId, bool isPickList);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool ChangeAutomaticZOrTabOrder(int ObjectHandle);
         #endregion

         #region Tree Wrapper Methods Import

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool OpenTree(int SecondaryObjectHandle, int SecondaryType, int ParentElementType, int ParentElementIsn);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CloseTree(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         [return: MarshalAs(UnmanagedType.I1)]
         public static extern bool InsertNode(int ObjectHandle, int index, bool isRoot);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool LeaveNode(int ObjectHandle, bool isRoot);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool GetNodeData(int ObjectHandle, out IntPtr line);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool SetNodeData(int ObjectHandle, IntPtr line, IntPtr newVal);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GetChildrenCount(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CreateChildNode(int ObjectHandle, IntPtr parentNodeInfo, IntPtr defaultNodeValue, out IntPtr childNodeInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CreateParentNode(int ObjectHandle, IntPtr childNodeInfo, int childNodeHashCode, IntPtr defaultNodeValue, out IntPtr parentNodeInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool DeleteNode(int ObjectHandle, IntPtr parentNodeInfo, int nodeIndexInParent);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CopyNode(int ObjectHandle, IntPtr nodeInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CutNode(int ObjectHandle, IntPtr nodeInfo);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsInCutState(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ResetIsInCutState(int ObjectHandle);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool PasteNode(int ObjectHandle, int pasteType, IntPtr nodeInfo, out IntPtr newRootPastedNode);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsCopyAllowed(int ObjectHandle, IntPtr nodeInfo, out IntPtr returnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsCutAllowed(int ObjectHandle, IntPtr nodeInfo, out IntPtr returnResult);



         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsPasteAllowed(int ObjectHandle, IntPtr nodeInfo, int pasteType, out IntPtr returnResult);


         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool RefreshNodeData(int ObjectHandle, IntPtr NodeInfo, out IntPtr result);

         #endregion

         #region Validation

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr ExpCompatible(int ObjectHandle, int isn, IntPtr mgAttrItem);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern char ExpAttr(int ObjectHandle, int expressionIndex, out IntPtr result);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool CheckCallProgram(int isn);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ChkExpRTFResult(int ObjectHandle, int expIdx, int isRTFEdit, int caseAllowed, IntPtr mgAttrItem, out IntPtr result);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool ExpMustExecOnClient(int ObjectHandle, int expressionIsn);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void CheckerInitialize(int ObjectHandle);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void CheckerTerminate(int ObjectHandle);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern int GetUnusedExpressionIndices(int ObjectHandle, out IntPtr unusedExpIndicesPtr);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsDotNetTypeAssignable(IntPtr mgAttrItemTo, IntPtr mgAttrItemFrom);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr GetDotNetMemberType(IntPtr objectType, IntPtr memberName);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern bool IsDotNetTypeControl(IntPtr typeName);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void ParseExpressionText(IntPtr str, out IntPtr result);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void SetCurrentTaskAsActive(int ObjectHandle);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern void GetAutoCompletionList(IntPtr str, out IntPtr result);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr GetFunctionSignatureText(IntPtr str);

         [DllImport(StudioDll, CallingConvention = CallingConvention.Cdecl)]
         public static extern IntPtr GetRtfFromText(IntPtr str);
         #endregion
      }
   }
}
