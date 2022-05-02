/*
*	Copyright (c) 2017-2021. RainyRizzle. All rights reserved
*	Contact to : https://www.rainyrizzle.com/ , contactrainyrizzle@gmail.com
*
*	This file is part of [AnyPortrait].
*
*	AnyPortrait can not be copied and/or distributed without
*	the express perission of [Seungjik Lee] of [RainyRizzle team].
*
*	It is illegal to download files from other than the Unity Asset Store and RainyRizzle homepage.
*	In that case, the act could be subject to legal sanctions.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditor;
//using UnityEngine.Profiling;

#if UNITY_2017_1_OR_NEWER
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif

using AnyPortrait;

namespace AnyPortrait
{

	[CustomEditor(typeof(apPortrait))]
	public class apInspector_Portrait : Editor
	{
		private apPortrait _targetPortrait = null;
		private apControlParam.CATEGORY _curControlCategory = apControlParam.CATEGORY.Etc;
		private bool _showBaseInspector = false;
		private List<apControlParam> _controlParams = null;

		//private bool _isFold_BasicSettings = false;
		private bool _isFold_RootPortraits = false;
		private bool _isFold_AnimationClips = false;
		private bool _isFold_AnimationEvents = false;
		private bool _isFold_Sockets = false;

		//추가 3.4
#if UNITY_2017_1_OR_NEWER
		private bool _isFold_Timeline = false;
		private int _nTimelineTrackSet = 0;
#endif
		//private bool _isFold_ConrolParameters = false;

		//3.7 추가 : 이미지들
		private bool _isImageLoaded = false;
		private Texture2D _img_EditorIsOpen = null;

		private Texture2D _img_OpenEditor = null;
		private Texture2D _img_QuickBake = null;
		private Texture2D _img_RefreshMeshes = null;
		private Texture2D _img_PrefabAsset = null;

		private Texture2D _img_BasicSettings = null;
		private Texture2D _img_Prefab = null;
		private Texture2D _img_RootPortraits = null;
		private Texture2D _img_AnimationSettings = null;
		private Texture2D _img_Mecanim = null;
#if UNITY_2017_1_OR_NEWER
		private Texture2D _img_Timeline = null;
#endif
		private Texture2D _img_ControlParams = null;

		//추가 21.8.2 : 애니메이션 이벤트도 Inspector에 표시하자
		private Texture2D _img_AnimEvents = null;
		private Texture2D _img_AnimEvents_UnityVer = null;
		private Texture2D _img_Copy = null;
		//private Texture2D _img_CopyAll = null;
		private Texture2D _img_Sockets = null;

		private Texture2D _img_AddButton = null;
		private Texture2D _img_RemoveButton = null;

		private GUIContent _guiContent_EditorIsOpen = null;
		private GUIContent _guiContent_OpenEditor = null;
		private GUIContent _guiContent_QuickBake = null;
		private GUIContent _guiContent_RefreshMeshes = null;
		private GUIContent _guiContent_PrefabAsset = null;

		private GUIContent _guiContent_BasicSettings = null;
		private GUIContent _guiContent_Prefab = null;
		private GUIContent _guiContent_RootPortraits = null;
		private GUIContent _guiContent_AnimationSettings = null;

		private GUIContent _guiContent_Mecanim = null;
#if UNITY_2017_1_OR_NEWER
		private GUIContent _guiContent_Timeline = null;
#endif

		private GUIContent _guiContent_ControlParams = null;

		private GUIContent _guiContent_AnimEvents = null;
		private GUIContent _guiContent_AnimEvents_UnityVer = null;
		private GUIContent _guiContent_Copy = null;
		//private GUIContent _guiContent_CopyAll = null;
		private GUIContent _guiContent_Sockets = null;

		private GUIContent _guiContent_AddButton = null;
		private GUIContent _guiContent_RemoveButton = null;

		
		private GUIStyle _guiStyle_buttonIcon = null;
		private GUIStyle _guiStyle_subTitle = null;
		private GUIStyle _guiStyle_subBox = null;
		private GUIStyle _guiStyle_button_NoMargin = null;
		private GUIStyle _guiStyle_button_Margin6 = null;
		private GUIStyle _guiStyle_WhiteText = null;
		
		private apGUIContentWrapper _guiContent_Category = null;
		

		//추가 20.4.21 : 이미지 경로
		private string _basePath = "";


		//추가 20.9.14 : 대상 Portrait의 프리팹 여부
		private bool _isPrefabAsset = false;
		private bool _isPrefabInstance = false;
		private apEditorUtil.PREFAB_STATUS _prefabStatus = apEditorUtil.PREFAB_STATUS.NoPrefab;
		private UnityEngine.Object _srcPrefabObject = null;
		private GameObject _rootGameObjAsPrefabInstance = null;

		//추가 21.8.21 : 소켓 정보들
		private enum SOCKET_TYPE
		{
			Transform,
			Bone
		}
		private class SocketInfo
		{
			public string _name = null;
			public Transform _targetTransform = null;
			public SOCKET_TYPE _type = SOCKET_TYPE.Transform;

			public SocketInfo(apOptTransform socketParentTransform)
			{
				_name = socketParentTransform._name;
				_targetTransform = socketParentTransform._socketTransform;
				_type = SOCKET_TYPE.Transform;
			}

			public SocketInfo(apOptBone socketParentBone)
			{
				_name = socketParentBone._name;
				_targetTransform = socketParentBone._socketTransform;
				_type = SOCKET_TYPE.Bone;
			}
		}

		private List<SocketInfo> _socketInfos = new List<SocketInfo>();


		void OnEnable()
		{
			_targetPortrait = null;

			//_isFold_BasicSettings = true;
			_isFold_RootPortraits = true;
			_isFold_AnimationClips = true;
			//_isFold_ConrolParameters = true;

			_isFold_AnimationEvents = true;
			_isFold_Sockets = true;

			//추가 3.4
#if UNITY_2017_1_OR_NEWER
			_isFold_Timeline = true;//<<
			_nTimelineTrackSet = 0;
#endif
		}

		private void LoadImages()
		{
			
			if (_isImageLoaded)
			{
				return;
			}

			//이전
			//_basePath = apPathSetting.I.Load();
			
			//변경 21.10.4 : 함수 변경
			_basePath = apPathSetting.I.RefreshAndGetBasePath(true);//강제로 로드후 갱신

			_img_EditorIsOpen = LoadImage("InspectorIcon_EditorIsOpen");

			_img_OpenEditor = LoadImage("InspectorIcon_OpenEditor");
			_img_QuickBake = LoadImage("InspectorIcon_QuickBake");
			_img_RefreshMeshes = LoadImage("InspectorIcon_RefreshMeshes");
			_img_PrefabAsset = LoadImage("InspectorIcon_PrefabAsset");

			_img_BasicSettings = LoadImage("InspectorIcon_BasicSettings");
			_img_Prefab = LoadImage("InspectorIcon_Prefab");
			_img_RootPortraits = LoadImage("InspectorIcon_RootPortraits");
			_img_AnimationSettings = LoadImage("InspectorIcon_AnimationSettings");
			_img_Mecanim = LoadImage("InspectorIcon_Mecanim");
#if UNITY_2017_1_OR_NEWER
			_img_Timeline = LoadImage("InspectorIcon_Timeline");
#endif
			_img_ControlParams = LoadImage("InspectorIcon_ControlParams");
			_img_AnimEvents = LoadImage("InspectorIcon_AnimEvent");
			_img_AnimEvents_UnityVer = LoadImage("InspectorIcon_AnimEvent_UnityVer");
			_img_Copy = LoadImage("InspectorIcon_Copy");
			//_img_CopyAll = LoadImage("InspectorIcon_CopyAll");
			_img_Sockets = LoadImage("InspectorIcon_Sockets");

			_img_AddButton = LoadImage("InspectorIcon_AddButton");
			_img_RemoveButton = LoadImage("InspectorIcon_RemoveButton");

			


			_guiContent_EditorIsOpen = new GUIContent("  Editor is opened", _img_EditorIsOpen);
			_guiContent_OpenEditor = new GUIContent(_img_OpenEditor);
			_guiContent_QuickBake = new GUIContent(_img_QuickBake);
			_guiContent_RefreshMeshes = new GUIContent(_img_RefreshMeshes);
			_guiContent_PrefabAsset = new GUIContent("  Prefab Asset is selected", _img_PrefabAsset);

			_guiContent_BasicSettings = new GUIContent("  Basic Settings", _img_BasicSettings);
			_guiContent_Prefab = new GUIContent("  Prefab", _img_Prefab);
			_guiContent_RootPortraits = new GUIContent("  Root Portraits", _img_RootPortraits);
			_guiContent_AnimationSettings = new GUIContent("  Animation Settings", _img_AnimationSettings);

			_guiContent_Mecanim = new GUIContent("  Mecanim Settings", _img_Mecanim);
#if UNITY_2017_1_OR_NEWER
			_guiContent_Timeline = new GUIContent("  Timeline Settings", _img_Timeline);
#endif

			_guiContent_ControlParams = new GUIContent("  Control Parameters", _img_ControlParams);
			_guiContent_AnimEvents = new GUIContent("  Animation Events", _img_AnimEvents);
			_guiContent_AnimEvents_UnityVer = new GUIContent("  Animation Events", _img_AnimEvents_UnityVer);
			_guiContent_Copy = new GUIContent(_img_Copy);
			//_guiContent_CopyAll = new GUIContent(_img_CopyAll);
			_guiContent_Sockets = new GUIContent("  Sockets", _img_Sockets);

			_guiContent_AddButton = new GUIContent(_img_AddButton);
			_guiContent_RemoveButton = new GUIContent(_img_RemoveButton);
			
			_guiStyle_buttonIcon = new GUIStyle(GUI.skin.label);
			_guiStyle_buttonIcon.alignment = TextAnchor.MiddleCenter;

			_guiStyle_button_NoMargin = new GUIStyle(GUI.skin.button);
			_guiStyle_button_NoMargin.alignment = TextAnchor.MiddleCenter;
			_guiStyle_button_NoMargin.margin = new RectOffset(0, 0, 2, 2);
			_guiStyle_button_NoMargin.padding = new RectOffset(0, 0, 1, 1);

			_guiStyle_button_Margin6 = new GUIStyle(GUI.skin.button);
			_guiStyle_button_Margin6.alignment = TextAnchor.MiddleCenter;
			_guiStyle_button_Margin6.margin = new RectOffset(0, 0, 6, 6);
			_guiStyle_button_Margin6.padding = new RectOffset(0, 0, 1, 1);

			_guiStyle_subTitle = new GUIStyle(GUI.skin.box);
			_guiStyle_subTitle.alignment = TextAnchor.MiddleCenter;
			_guiStyle_subTitle.margin = new RectOffset(0, 0, 0, 0);
			_guiStyle_subTitle.padding = new RectOffset(0, 0, 0, 0);

			_guiStyle_subBox = new GUIStyle(GUI.skin.box);
			_guiStyle_subBox.alignment = TextAnchor.MiddleCenter;
			_guiStyle_subBox.padding = new RectOffset(0, 0, 2, 2);

			_guiStyle_WhiteText = new GUIStyle(GUI.skin.label);
			_guiStyle_WhiteText.normal.textColor = Color.white;

			_isImageLoaded = true;
		}

		public override void OnInspectorGUI()
		{
			//return;
			LoadImages();
			

			//base.OnInspectorGUI();
			apPortrait targetPortrait = target as apPortrait;

			if (targetPortrait != _targetPortrait)
			{
				_targetPortrait = targetPortrait;
				Init();
				FindSockets();
			}
			if (_targetPortrait == null)
			{
				//Profiler.EndSample();
				return;
			}

			//Profiler.BeginSample("anyPortrait Inspector GUI");


			//return;
			if (apEditor.IsOpen())
			{
				//에디터가 작동중에는 안보이도록 하자
				//EditorGUILayout.LabelField("Editor is opened");
				GUILayout.Space(10);
				
				EditorGUILayout.LabelField(_guiContent_EditorIsOpen, GUILayout.Height(36));

				//Profiler.EndSample();

				return;
			}

			try
			{
				bool request_OpenEditor = false;
				bool request_QuickBake = false;
				bool request_RefreshMeshes = false;
				bool prevImportant = _targetPortrait._isImportant;
				//MonoBehaviour prevAnimEventListener = _targetPortrait._optAnimEventListener;
				int prevSortingLayerID = _targetPortrait._sortingLayerID;
				apPortrait.SORTING_ORDER_OPTION prevSortingOrderOption = _targetPortrait._sortingOrderOption;
				int prevSortingOrder = _targetPortrait._sortingOrder;
				int prevOrderPerDepth = _targetPortrait._sortingOrderPerDepth;//추가 21.1.31

				if (_isPrefabAsset)
				{
					//추가 20.9.15 : 만약 프리팹 에셋이라면 에디터를 열 수 없다.
					GUILayout.Space(10);
				
					EditorGUILayout.LabelField(_guiContent_PrefabAsset, GUILayout.Height(36));

					Color prevBackColor = GUI.backgroundColor;
							GUI.backgroundColor = new Color(1.0f, 0.7f, 0.7f, 1.0f);
							GUILayout.Box("Prefab Assets cannot be edited.\nPlace the Prefab in the Scene as an Instance.",
								_guiStyle_subTitle,
								GUILayout.Width((int)EditorGUIUtility.currentViewWidth - 36), GUILayout.Height(40));
							GUI.backgroundColor = prevBackColor;
				}
				else
				{
					if (!EditorApplication.isPlaying)
					{
						int iconWidth = 32;
						int iconHeight = 34;
						int buttonHeight = 34;

						//추가 19.5.26 : 용량 최적화 기능이 추가되었는가
						if (!_targetPortrait._isSizeOptimizedV117)
						{
							GUILayout.Space(10);

							Color prevBackColor = GUI.backgroundColor;
							GUI.backgroundColor = new Color(1.0f, 0.7f, 0.7f, 1.0f);
							GUILayout.Box("[Bake] was not executed.\nExecute the [Bake] again.",
								_guiStyle_subTitle,
								GUILayout.Width((int)EditorGUIUtility.currentViewWidth - 36), GUILayout.Height(40));
							GUI.backgroundColor = prevBackColor;
						}

						if (!_targetPortrait._isOptimizedPortrait)
						{
							GUILayout.Space(10);

							EditorGUILayout.BeginHorizontal(GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							EditorGUILayout.LabelField(_guiContent_OpenEditor, _guiStyle_buttonIcon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							if (GUILayout.Button("Open Editor and Select", GUILayout.Height(buttonHeight)))
							{
								request_OpenEditor = true;
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.BeginHorizontal(GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							EditorGUILayout.LabelField(_guiContent_QuickBake, _guiStyle_buttonIcon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							if (GUILayout.Button("Quick Bake", GUILayout.Height(buttonHeight)))
							{
								request_QuickBake = true;
							}
							EditorGUILayout.EndHorizontal();
						}
						else
						{
							GUILayout.Space(10);

							EditorGUILayout.BeginHorizontal(GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							EditorGUILayout.LabelField(_guiContent_OpenEditor, _guiStyle_buttonIcon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
							GUILayout.Space(5);
							if (GUILayout.Button("Open Editor (Not Selectable)", GUILayout.Height(buttonHeight)))
							{
								//열기만 하고 선택은 못함
								request_OpenEditor = true;
							}
							EditorGUILayout.EndHorizontal();
						}
						//추가 12.18 : Mesh를 리프레시 하자

						EditorGUILayout.BeginHorizontal(GUILayout.Height(iconHeight));
						GUILayout.Space(5);
						EditorGUILayout.LabelField(_guiContent_RefreshMeshes, _guiStyle_buttonIcon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
						GUILayout.Space(5);
						if (GUILayout.Button("Refresh Meshes", GUILayout.Height(buttonHeight)))
						{
							request_RefreshMeshes = true;
						}
						EditorGUILayout.EndHorizontal();


					}
				}
				

				GUILayout.Space(10);

				
				int width = (int)EditorGUIUtility.currentViewWidth;
				int subTitleWidth = width - 44;
				int subTitleHeight = 26;


				//BasicSettings
				//-----------------------------------------------------------------------------
				//"Basic Settings"
				
				
				
				GUILayout.Box(_guiContent_BasicSettings, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));


				_targetPortrait._isImportant = EditorGUILayout.Toggle("Is Important", _targetPortrait._isImportant);
				
				//이동 21.9.25 : 애니메이션 이벤트 항목으로 옮긴다.
				//_targetPortrait._optAnimEventListener = (MonoBehaviour)EditorGUILayout.ObjectField("Event Listener", _targetPortrait._optAnimEventListener, typeof(MonoBehaviour), true);


				GUILayout.Space(5);
				//추가3.22
				//Sorting Layer
				string[] sortingLayerName = new string[SortingLayer.layers.Length];
				int layerIndex = -1;
				for (int i = 0; i < SortingLayer.layers.Length; i++)
				{
					sortingLayerName[i] = SortingLayer.layers[i].name;
					if (SortingLayer.layers[i].id == _targetPortrait._sortingLayerID)
					{
						layerIndex = i;
					}
				}
				int nextLayerIndex = EditorGUILayout.Popup("Sorting Layer", layerIndex, sortingLayerName);
				apPortrait.SORTING_ORDER_OPTION nextSortingOption = (apPortrait.SORTING_ORDER_OPTION)EditorGUILayout.EnumPopup("Sorting Order Option", _targetPortrait._sortingOrderOption);

				int nextLayerOrder = _targetPortrait._sortingOrder;
				if (_targetPortrait._sortingOrderOption == apPortrait.SORTING_ORDER_OPTION.SetOrder)
				{
					nextLayerOrder = EditorGUILayout.IntField("Sorting Order", _targetPortrait._sortingOrder);

					if (nextLayerOrder != _targetPortrait._sortingOrder)
					{
						_targetPortrait.SetSortingOrder(nextLayerOrder);
					}
				}
				else if(_targetPortrait._sortingOrderOption == apPortrait.SORTING_ORDER_OPTION.DepthToOrder 
					|| _targetPortrait._sortingOrderOption == apPortrait.SORTING_ORDER_OPTION.ReverseDepthToOrder)
				{
					//추가 21.1.31 : Depth To Order일때, 1씩만 증가하는게 아닌 더 큰값으로 증가할 수도 있게 만들자
					int nextOrderPerDepth = EditorGUILayout.IntField("Order Per Depth", _targetPortrait._sortingOrderPerDepth);
					if(nextOrderPerDepth != _targetPortrait._sortingOrderPerDepth)
					{
						if(nextOrderPerDepth < 1)
						{
							nextOrderPerDepth = 1;
						}

						_targetPortrait._sortingOrderPerDepth = nextOrderPerDepth;

						//변경된 Sorting Order Option에 따라서 바로 Sorting을 해야한다.
						_targetPortrait.ApplySortingOptionToOptRootUnits();
					}
				}
				

				if (nextLayerIndex != layerIndex)
				{
					//Sorting Layer를 바꾸자
					if (nextLayerIndex >= 0 && nextLayerIndex < SortingLayer.layers.Length)
					{
						string nextLayerName = SortingLayer.layers[nextLayerIndex].name;
						_targetPortrait.SetSortingLayer(nextLayerName);
					}
				}
				if(nextSortingOption != _targetPortrait._sortingOrderOption)
				{
					_targetPortrait._sortingOrderOption = nextSortingOption;
					//변경된 Sorting Order Option에 따라서 바로 Sorting을 해야한다.
					_targetPortrait.ApplySortingOptionToOptRootUnits();

					switch (_targetPortrait._sortingOrderOption)
					{
						case apPortrait.SORTING_ORDER_OPTION.SetOrder:
							_targetPortrait.SetSortingOrder(_targetPortrait._sortingOrder);
							break;

						case apPortrait.SORTING_ORDER_OPTION.DepthToOrder:
						case apPortrait.SORTING_ORDER_OPTION.ReverseDepthToOrder:
							_targetPortrait.SetSortingOrderChangedAutomatically(true);
							_targetPortrait.RefreshSortingOrderByDepth();
							break;
					}
				}
				



				if (prevImportant != _targetPortrait._isImportant ||
					prevSortingLayerID != _targetPortrait._sortingLayerID ||
					prevSortingOrderOption != _targetPortrait._sortingOrderOption ||
					prevSortingOrder != _targetPortrait._sortingOrder ||
					prevOrderPerDepth != _targetPortrait._sortingOrderPerDepth)
				{
					apEditorUtil.SetEditorDirty();
				}


				GUILayout.Space(5);

				//빌보드
				apPortrait.BILLBOARD_TYPE nextBillboard = (apPortrait.BILLBOARD_TYPE)EditorGUILayout.EnumPopup("Billboard Type", _targetPortrait._billboardType);
				if (nextBillboard != _targetPortrait._billboardType)
				{
					_targetPortrait._billboardType = nextBillboard;
					apEditorUtil.SetEditorDirty();
				}

				GUILayout.Space(20);



				// Prefab (유효한 경우만)
				//-----------------------------------------------------------------------------
				if (_isPrefabInstance)
				{	
					GUILayout.Box(_guiContent_Prefab, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));

					
					Color prevBackColor = GUI.backgroundColor;

					//연결 상태를 보여주자
					string strStatus = null;
					switch(_prefabStatus)
					{
						case apEditorUtil.PREFAB_STATUS.Connected:
							GUI.backgroundColor = new Color(0.7f, 1.0f, 1.0f, 1.0f);
							strStatus = "Source Prefab";
							break;

						case apEditorUtil.PREFAB_STATUS.Disconnected:
							GUI.backgroundColor = new Color(1.0f, 0.7f, 0.7f, 1.0f);
							strStatus = "(Disconnected)";
							break;

						case apEditorUtil.PREFAB_STATUS.Missing:
						default:
							GUI.backgroundColor = new Color(1.0f, 0.7f, 0.7f, 1.0f);
							strStatus = "(Missing)";
							break;
					}
					EditorGUILayout.ObjectField(strStatus, _srcPrefabObject, typeof(UnityEngine.Object), false);
					EditorGUILayout.ObjectField("Root GameObject", _rootGameObjAsPrefabInstance, typeof(GameObject), false);
					GUI.backgroundColor = prevBackColor;

					int width_PrefabButtons = (width / 2) - 24;

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(5);
					if (GUILayout.Button("Apply", GUILayout.Width(width_PrefabButtons)))
					{
						//프리팹 변경 내용을 저장하자
						apEditorUtil.ApplyPrefab(_targetPortrait);
						RefreshPrefabStatus();
					}
					if (GUILayout.Button("Refresh", GUILayout.Width(width_PrefabButtons)))
					{
						//프리팹 연결 정보를 갱신한다.
						RefreshPrefabStatus();
					}

					
					EditorGUILayout.EndHorizontal();

					//Disconnect를 할 수 있다.
					//Legacy : 단순 Disconnect를 할 수 있다.
					//2018.3 : Disconnect를 한 후, 복원 정보를 모두 삭제할 수 있다.
#if UNITY_2018_3_OR_NEWER
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(5);
					
					if (GUILayout.Button("Disconnect", GUILayout.Width(width_PrefabButtons)))
					{
						//Disconnect를 하되, 연결 정보는 남겨둔다.
						if(EditorUtility.DisplayDialog(
												"Disconnecting from Prefab", 
												"Are you sure you want to disconnect this Portrait from the Prefab Asset?", 
												"Disconnect", "Cancel"))
						{
							apEditorUtil.CheckAndRefreshPrefabInfo(_targetPortrait);
							apEditorUtil.DisconnectPrefab(_targetPortrait);
							RefreshPrefabStatus();
						}
						
					}

					if (GUILayout.Button("Clear", GUILayout.Width(width_PrefabButtons)))
					{
						//Disconnect를 하고, 연결 정보를 삭제한다.
						if(EditorUtility.DisplayDialog(
												"Disconnecting from Prefab", 
												"Are you sure you want to disconnect this Portrait from the Prefab Asset?\nThis completely deletes the connection data with the Prefab.", 
												"Disconnect and Clear", "Cancel"))
						{
							apEditorUtil.DisconnectPrefab(_targetPortrait, true);
							RefreshPrefabStatus();
						}
					}
					EditorGUILayout.EndHorizontal();
#else
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(5);
					if (GUILayout.Button("Disconnect", GUILayout.Width(width - 45)))
					{
						//Disconnect를 하되, 연결 정보는 남겨둔다.
						if(EditorUtility.DisplayDialog(
												"Disconnecting from Prefab", 
												"Are you sure you want to disconnect this Portrait from the Prefab Asset?", 
												"Disconnect", "Cancel"))
						{
							apEditorUtil.CheckAndRefreshPrefabInfo(_targetPortrait);
							apEditorUtil.DisconnectPrefab(_targetPortrait);
							RefreshPrefabStatus();
						}
					}
					EditorGUILayout.EndHorizontal();
#endif

					GUILayout.Space(20);
				}
				



				// Root Portraits
				//-----------------------------------------------------------------------------
				GUILayout.Box(_guiContent_RootPortraits, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));

				_isFold_RootPortraits = EditorGUILayout.Foldout(_isFold_RootPortraits, "Portraits");
				if(_isFold_RootPortraits)
				{
					for (int i = 0; i < _targetPortrait._optRootUnitList.Count; i++)
					{
						apOptRootUnit rootUnit = _targetPortrait._optRootUnitList[i];
						EditorGUILayout.ObjectField("[" + i + "]", rootUnit, typeof(apOptRootUnit), true);
					}
				}

				GUILayout.Space(20);


				//추가 21.8.21 : 소켓이 있다면 정보들을 출력한다.
				// Sockets (선택)
				//-----------------------------------------------------------------------------
				if(_socketInfos != null && _socketInfos.Count > 0)
				{
					GUILayout.Box(_guiContent_Sockets, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));
					_isFold_Sockets = EditorGUILayout.Foldout(_isFold_Sockets, "Sockets");
					if(_isFold_Sockets)
					{
						int nSockets = _socketInfos.Count;
						SocketInfo curInfo = null;
						for (int iInfo = 0; iInfo < nSockets; iInfo++)
						{
							curInfo = _socketInfos[iInfo];

							//이름 / Transform / 이름 복사

							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(5);

							EditorGUILayout.EnumPopup(curInfo._type, GUILayout.Width(80));

							EditorGUILayout.TextField(curInfo._name, GUILayout.Width(width - (10 + 120 + 6 + 80 + 36)));

							EditorGUILayout.ObjectField(curInfo._targetTransform, typeof(Transform), true, GUILayout.Width(120));

							EditorGUILayout.EndHorizontal();
						}
					}

					GUILayout.Space(20);
				}


				// Animation Settings
				//-----------------------------------------------------------------------------

				GUILayout.Box(_guiContent_AnimationSettings, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));

				_isFold_AnimationClips = EditorGUILayout.Foldout(_isFold_AnimationClips, "Animation Clips");

				int nAnimClips = _targetPortrait._animClips != null ? _targetPortrait._animClips.Count : 0;

				if(_isFold_AnimationClips)
				{
					if (nAnimClips > 0)
					{
						for (int i = 0; i < nAnimClips; i++)
						{
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(5);
							apAnimClip animClip = _targetPortrait._animClips[i];
							if (animClip._uniqueID == _targetPortrait._autoPlayAnimClipID)
							{
								EditorGUILayout.LabelField("[" + i + "] (Auto)", GUILayout.Width(80));
							}
							else
							{
								EditorGUILayout.LabelField("[" + i + "]", GUILayout.Width(80));
							}
							EditorGUILayout.TextField(animClip._name);
							try
							{
								AnimationClip nextAnimationClip = EditorGUILayout.ObjectField(animClip._animationClipForMecanim, typeof(AnimationClip), false) as AnimationClip;
								if (nextAnimationClip != animClip._animationClipForMecanim)
								{
									UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
									Undo.IncrementCurrentGroup();
									Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Changed");

									animClip._animationClipForMecanim = nextAnimationClip;
								}
							}
							catch (Exception)
							{ }

							EditorGUILayout.EndHorizontal();
						}
					}
				}

				GUILayout.Space(10);

				AnimationClip nextEmptyAnimClip = EditorGUILayout.ObjectField("Empty Anim Clip", _targetPortrait._emptyAnimClipForMecanim, typeof(AnimationClip), false) as AnimationClip;
				if (nextEmptyAnimClip != _targetPortrait._emptyAnimClipForMecanim)
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
					Undo.IncrementCurrentGroup();
					Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Changed");

					_targetPortrait._emptyAnimClipForMecanim = nextEmptyAnimClip;
				}

				GUILayout.Space(10);

				//EditorGUILayout.LabelField("Mecanim Settings");
				EditorGUILayout.LabelField(_guiContent_Mecanim, GUILayout.Height(24));
				
				bool isNextUsingMecanim = EditorGUILayout.Toggle("Use Mecanim", _targetPortrait._isUsingMecanim);
				if (_targetPortrait._isUsingMecanim != isNextUsingMecanim)
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
					Undo.IncrementCurrentGroup();
					Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

					_targetPortrait._isUsingMecanim = isNextUsingMecanim;
				}


				if(_targetPortrait._isUsingMecanim)
				{
					//GUILayout.Space(10);
					try
					{
						Animator nextAnimator = EditorGUILayout.ObjectField("Animator", _targetPortrait._animator, typeof(Animator), true) as Animator;
						if (nextAnimator != _targetPortrait._animator)
						{
							//하위에 있는 Component일 때에만 변동 가능
							if (nextAnimator == null)
							{
								_targetPortrait._animator = null;
							}
							else
							{
								if (nextAnimator == _targetPortrait.GetComponent<Animator>())
								{
									_targetPortrait._animator = nextAnimator;
								}
								else
								{
									EditorUtility.DisplayDialog("Invalid Animator", "Invalid Animator. Only the Animator, which is its own component, is valid.", "Okay");

								}
							}

						}
					}
					catch(Exception)
					{

					}
					if (_targetPortrait._animator == null)
					{
						//1. Animator가 없다면
						// > 생성하기
						// > 생성되어 있다면 다시 링크
						GUIStyle guiStyle_WarningText = new GUIStyle(GUI.skin.label);
						guiStyle_WarningText.normal.textColor = Color.red;
						EditorGUILayout.LabelField("Warning : No Animator!", guiStyle_WarningText);
						GUILayout.Space(5);

						if(GUILayout.Button("Add / Check Animator", GUILayout.Height(25)))
						{
							UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
							Undo.IncrementCurrentGroup();
							Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

							Animator animator = _targetPortrait.gameObject.GetComponent<Animator>();
							if(animator == null)
							{
								animator = _targetPortrait.gameObject.AddComponent<Animator>();
							}
							_targetPortrait._animator = animator;
						}
					}
					else
					{
						//2. Animator가 있다면
						if (GUILayout.Button("Refresh Layers", GUILayout.Height(25)))
						{
							UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
							Undo.IncrementCurrentGroup();
							Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

							//Animator의 Controller가 있는지 체크해야한다.
								
							if(_targetPortrait._animator.runtimeAnimatorController == null)
							{
								//AnimatorController가 없다면 Layer는 초기화
								_targetPortrait._animatorLayerBakedData.Clear();
							}
							else
							{
								//AnimatorController가 있다면 레이어에 맞게 설정
								_targetPortrait._animatorLayerBakedData.Clear();
								UnityEditor.Animations.AnimatorController animatorController = _targetPortrait._animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

								if(animatorController != null && animatorController.layers.Length > 0)
								{
									for (int iLayer = 0; iLayer < animatorController.layers.Length; iLayer++)
									{
										apAnimMecanimData_Layer newLayerData = new apAnimMecanimData_Layer();
										newLayerData._layerIndex = iLayer;
										newLayerData._layerName = animatorController.layers[iLayer].name;
										newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Unknown;
										switch (animatorController.layers[iLayer].blendingMode)
										{
											case UnityEditor.Animations.AnimatorLayerBlendingMode.Override:
												newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Override;
												break;

											case UnityEditor.Animations.AnimatorLayerBlendingMode.Additive:
												newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Additive;
												break;
										}

										_targetPortrait._animatorLayerBakedData.Add(newLayerData);
									}
								}
							}
						}
						GUILayout.Space(5);
						EditorGUILayout.LabelField("Animator Controller Layers");
						for (int i = 0; i < _targetPortrait._animatorLayerBakedData.Count; i++)
						{
							apAnimMecanimData_Layer layer = _targetPortrait._animatorLayerBakedData[i];
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(5);
							EditorGUILayout.LabelField("[" + layer._layerIndex + "]", GUILayout.Width(50));
							EditorGUILayout.TextField(layer._layerName);
							apAnimMecanimData_Layer.MecanimLayerBlendType nextBlendType = (apAnimMecanimData_Layer.MecanimLayerBlendType)EditorGUILayout.EnumPopup(layer._blendType);
							EditorGUILayout.EndHorizontal();

							if (nextBlendType != layer._blendType)
							{
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

								_targetPortrait._animatorLayerBakedData[i]._blendType = nextBlendType;
							}
						}
					}
						
				}


				GUILayout.Space(20);


				//추가 3.4 : 타임라인 설정
#if UNITY_2017_1_OR_NEWER

				EditorGUILayout.LabelField(_guiContent_Timeline, GUILayout.Height(24));

				_isFold_Timeline = EditorGUILayout.Foldout(_isFold_Timeline, "Track Data");
				if(_isFold_Timeline)
				{
					
					int nextTimelineTracks = EditorGUILayout.DelayedIntField("Size", _nTimelineTrackSet);
					if(nextTimelineTracks != _nTimelineTrackSet)
					{
						//TimelineTrackSet의 개수가 바뀌었다. 
						UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
						Undo.IncrementCurrentGroup();
						Undo.RegisterCompleteObjectUndo(_targetPortrait, "Track Setting Changed");
						_nTimelineTrackSet = nextTimelineTracks;
						if(_nTimelineTrackSet < 0)
						{
							_nTimelineTrackSet = 0;
						}

						//일단 이전 개수만큼 복사를 한다.
						int nPrev = 0;
						List<apPortrait.TimelineTrackPreset> prevSets = new List<apPortrait.TimelineTrackPreset>();
						if(targetPortrait._timelineTrackSets != null && targetPortrait._timelineTrackSets.Length > 0)
						{
							for (int i = 0; i < targetPortrait._timelineTrackSets.Length; i++)
							{
								prevSets.Add(targetPortrait._timelineTrackSets[i]);
							}
							nPrev = targetPortrait._timelineTrackSets.Length;
						}
						
						//배열을 새로 만들자
						targetPortrait._timelineTrackSets = new apPortrait.TimelineTrackPreset[_nTimelineTrackSet];

						//가능한 이전 소스를 복사한다.
						for (int i = 0; i < _nTimelineTrackSet; i++)
						{
							if(i < nPrev)
							{
								targetPortrait._timelineTrackSets[i] = new apPortrait.TimelineTrackPreset();
								targetPortrait._timelineTrackSets[i]._playableDirector = prevSets[i]._playableDirector;
								targetPortrait._timelineTrackSets[i]._trackName = prevSets[i]._trackName;
								targetPortrait._timelineTrackSets[i]._layer = prevSets[i]._layer;
								targetPortrait._timelineTrackSets[i]._blendMethod = prevSets[i]._blendMethod;
							}
							else
							{
								targetPortrait._timelineTrackSets[i] = new apPortrait.TimelineTrackPreset();
							}
						}


						apEditorUtil.ReleaseGUIFocus();
						
					}

					GUILayout.Space(5);

					if(targetPortrait._timelineTrackSets != null)
					{
						apPortrait.TimelineTrackPreset curTrackSet = null;
						for (int i = 0; i < targetPortrait._timelineTrackSets.Length; i++)
						{
							//트랙을 하나씩 적용
							curTrackSet = targetPortrait._timelineTrackSets[i];
							
							EditorGUILayout.LabelField("[" + i + "] : " + (curTrackSet._playableDirector == null ? "<None>" : curTrackSet._playableDirector.name));
							PlayableDirector nextDirector = EditorGUILayout.ObjectField("Director", curTrackSet._playableDirector, typeof(PlayableDirector), true) as PlayableDirector;
							string nextTrackName = EditorGUILayout.DelayedTextField("Track Name", curTrackSet._trackName);
							int nextLayer = EditorGUILayout.DelayedIntField("Layer", curTrackSet._layer);
							apAnimPlayUnit.BLEND_METHOD nextBlendMethod = (apAnimPlayUnit.BLEND_METHOD)EditorGUILayout.EnumPopup("Blend", curTrackSet._blendMethod);

							if(nextDirector != curTrackSet._playableDirector 
								|| nextTrackName != curTrackSet._trackName
								|| nextLayer != curTrackSet._layer
								|| nextBlendMethod != curTrackSet._blendMethod
								)
							{
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Track Setting Changed");

								curTrackSet._playableDirector = nextDirector;
								curTrackSet._trackName = nextTrackName;
								curTrackSet._layer = nextLayer;
								curTrackSet._blendMethod = nextBlendMethod;

								apEditorUtil.ReleaseGUIFocus();
							}

							GUILayout.Space(5);
						}
					}
				}

				GUILayout.Space(20);
#endif
				

				
				//추가 21.8.2 : 애니메이션 이벤트들을 표시한다. (존재하는 경우)
				//전체 애니메이션 개수를 먼저 계산하자
				int nTotalAnimEvents = 0;
				List<AnimEventInfo> animEventInfos = null;
				List<string> animEventNames = null;
				if (nAnimClips > 0)
				{
					List<apAnimEvent> curEvents = null;
					for (int i = 0; i < nAnimClips; i++)
					{
						curEvents = _targetPortrait._animClips[i]._animEvents;
						int nCurEvents = curEvents != null ? curEvents.Count : 0;
						if(nCurEvents == 0)
						{
							continue;
						}

						if(animEventInfos == null)
						{
							animEventInfos = new List<AnimEventInfo>();
						}
						if(animEventNames == null)
						{
							animEventNames = new List<string>();
						}

						for (int iEvent = 0; iEvent < nCurEvents; iEvent++)
						{
							apAnimEvent curEvent = curEvents[iEvent];
							AnimEventInfo eventInfo = MakeAnimEventInfo(curEvent);
							
							//해당 이름이 이미 있는지 확인하고, 없으면 이벤트 등록
							if(!animEventNames.Contains(eventInfo._strEventName))
							{
								animEventInfos.Add(eventInfo);
								animEventNames.Add(eventInfo._strEventName);
								nTotalAnimEvents += 1;
							}
						}
					}
				}




				//애니메이션 이벤트
				GUILayout.Box(
					_targetPortrait._animEventCallMode == apPortrait.ANIM_EVENT_CALL_MODE.SendMessage ? _guiContent_AnimEvents : _guiContent_AnimEvents_UnityVer, 
					_guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));

				Color prevColor = GUI.backgroundColor;

				//옵션 : 이벤트 리스너 모드부터 설정 (21.9.25)
				apPortrait.ANIM_EVENT_CALL_MODE nextAnimCallMode = (apPortrait.ANIM_EVENT_CALL_MODE)EditorGUILayout.EnumPopup("Event Method", _targetPortrait._animEventCallMode);
				if(nextAnimCallMode != _targetPortrait._animEventCallMode)
				{
					Undo.IncrementCurrentGroup();
					Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Setting Changed");

					_targetPortrait._animEventCallMode = nextAnimCallMode;

					//변경 되었을 때도 Event Wrapper Bake를 해야한다.
					if(_targetPortrait._unityEventWrapper == null)
					{
						_targetPortrait._unityEventWrapper = new apUnityEventWrapper();
					}
					_targetPortrait._unityEventWrapper.Bake(_targetPortrait);

					apEditorUtil.SetEditorDirty();
				}

				if (_targetPortrait._animEventCallMode == apPortrait.ANIM_EVENT_CALL_MODE.SendMessage)
				{
					GUILayout.Space(5);

					//이벤트 모드 1 : SendMessage 방식
					MonoBehaviour nextEventListener = (MonoBehaviour)EditorGUILayout.ObjectField("Event Listener", _targetPortrait._optAnimEventListener, typeof(MonoBehaviour), true);

					if (nextEventListener != _targetPortrait._optAnimEventListener)
					{
						Undo.IncrementCurrentGroup();
						Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Setting Changed");

						_targetPortrait._optAnimEventListener = nextEventListener;

						apEditorUtil.SetEditorDirty();
					}

					//애니메이션 이벤트가 존재하는 경우
					if (nTotalAnimEvents > 0 && animEventInfos != null)
					{
						GUILayout.Space(5);

						_isFold_AnimationEvents = EditorGUILayout.Foldout(_isFold_AnimationEvents, "Events");
						if (_isFold_AnimationEvents)
						{
							for (int iInfo = 0; iInfo < nTotalAnimEvents; iInfo++)
							{
								apAnimEvent curEvent = animEventInfos[iInfo]._animEvent;

								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(5);

								EditorGUILayout.TextField(animEventInfos[iInfo]._strEventName, GUILayout.Width(width - (10 + 35 + 4 + 36)));

								if (GUILayout.Button(_guiContent_Copy, _guiStyle_button_NoMargin, GUILayout.Width(35), GUILayout.Height(16)))
								{
									CopyAnimEventAsScript(curEvent);
								}

								EditorGUILayout.EndHorizontal();
							}
							if (GUILayout.Button("Copy All Events to Clipboard"))
							{
								//모든 이벤트 복사하기
								CopyAnimEventsAsScript(animEventInfos);
							}
						}
					}

					GUILayout.Space(20);
				}
				else
				{
					//이벤트 모드 2 : Unity Event 방식
					int nUnityEvents = 0;
					if (_targetPortrait._unityEventWrapper != null)
					{
						nUnityEvents = _targetPortrait._unityEventWrapper._unityEvents != null ? _targetPortrait._unityEventWrapper._unityEvents.Count : 0;
					}

					GUILayout.Space(5);

					_isFold_AnimationEvents = EditorGUILayout.Foldout(_isFold_AnimationEvents, "Events");
					if (_isFold_AnimationEvents && nUnityEvents > 0)
					{
						GUILayout.Space(5);
						apUnityEvent curUnityEvent = null;
						//SerializedProperty curEventProperty = null;
						apUnityEvent.TargetMethodSet curTMSet = null;
						apUnityEvent.TargetMethodSet removeTMSet = null;
						for (int iUEvent = 0; iUEvent < nUnityEvents; iUEvent++)
						{
							curUnityEvent = _targetPortrait._unityEventWrapper._unityEvents[iUEvent];

							//유니티 이벤트들을 GUI에 출력하자
							serializedObject.Update();

							#region [미사용 코드]
							//UnityEvent 오버라이드된 GUI 방식 (사용 안함)
							//curEventProperty = curUnityEvent.GetSerializedProperty(serializedObject, iUEvent);

							//if (curEventProperty != null)
							//{
							//	EditorGUILayout.LabelField(curUnityEvent.GetGUILabel());
							//	EditorGUI.BeginChangeCheck();

							//	try
							//	{
							//		EditorGUILayout.PropertyField(curEventProperty);
							//	}
							//	catch (Exception) { }

							//	if(EditorGUI.EndChangeCheck())
							//	{
							//		//Debug.Log("값이 바뀜");
							//		apEditorUtil.SetEditorDirty();
							//		serializedObject.ApplyModifiedProperties();
							//	}

							//} 
							#endregion

							//직접 UI를 만들자
							//배경

							if (curUnityEvent._targetMethods == null)
							{
								curUnityEvent._targetMethods = new List<apUnityEvent.TargetMethodSet>();
							}
							int nTMs = curUnityEvent._targetMethods.Count;
							removeTMSet = null;//삭제 확인용

							Rect lastRect = GUILayoutUtility.GetLastRect();


							GUI.backgroundColor = new Color(prevColor.r * 0.5f, prevColor.g * 0.5f, prevColor.b * 0.5f, 1.0f);

							int offsetY = 10;
							if (iUEvent == 0)
							{
								offsetY -= 5;
								
							}
							GUI.Box(new Rect(lastRect.x - 5, lastRect.y + offsetY, width - 35, 45 + (nTMs * 25)), "");
							
							GUI.backgroundColor = prevColor;
							
							
							EditorGUILayout.LabelField(curUnityEvent.GetGUILabel(), _guiStyle_WhiteText);

							int width_TM_Target = Mathf.Clamp((int)((float)width * 0.3f), 80, 200);
							int width_TM_Remove = 20;
							int width_TM_Method = width - (60 + width_TM_Target + width_TM_Remove);


							for (int iTM = 0; iTM < nTMs; iTM++)
							{
								curTMSet = curUnityEvent._targetMethods[iTM];

								EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
								GUILayout.Space(5);
								try
								{
									MonoBehaviour nextMonoTarget = curTMSet._target;
									nextMonoTarget = EditorGUILayout.ObjectField(curTMSet._target, typeof(MonoBehaviour), true, GUILayout.Width(width_TM_Target)) as MonoBehaviour;
									if (nextMonoTarget != curTMSet._target)
									{
										//Undo 등록
										//메소드 초기화 및 리스트 작성									
										Undo.IncrementCurrentGroup();
										Undo.RegisterCompleteObjectUndo(_targetPortrait, "Event Changed");

										curTMSet._target = nextMonoTarget;
										curTMSet._methodName = "";

										apEditorUtil.SetEditorDirty();
									}
								}
								catch (Exception) { }
								
								//Method들을 보여주자
								string methodName = string.IsNullOrEmpty(curTMSet._methodName) ? "" : curTMSet._methodName;

								
								if(GUILayout.Button(methodName, GUILayout.Width(width_TM_Method), GUILayout.Height(18)))
								{
									ShowMethodsOfUnityEvent(curUnityEvent, curTMSet);
								}

								if(GUILayout.Button(_guiContent_RemoveButton, _guiStyle_buttonIcon, GUILayout.Width(width_TM_Remove)))
								{
									removeTMSet = curTMSet;
								}
								EditorGUILayout.EndHorizontal();
								
							}
							//타겟, 함수, 삭제
							//추가
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(width - (67));
							if(GUILayout.Button(_guiContent_AddButton, _guiStyle_buttonIcon, GUILayout.Width(width_TM_Remove)))
							{
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Event Changed");

								curUnityEvent._targetMethods.Add(new apUnityEvent.TargetMethodSet());

								apEditorUtil.SetEditorDirty();
							}
							EditorGUILayout.EndHorizontal();
							
							if(removeTMSet != null)
							{
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Event Changed");

								curUnityEvent._targetMethods.Remove(removeTMSet);

								apEditorUtil.SetEditorDirty();

								removeTMSet = null;
							}


							GUILayout.Space(10);
						}
					}
					GUILayout.Space(20);
					
				}
				


				bool isChanged = false;

				// Control Parameters
				//-----------------------------------------------------------------------------

				if(_guiContent_Category == null)
				{
					_guiContent_Category = apGUIContentWrapper.Make("Category", false);
				}
				

				GUILayout.Box(_guiContent_ControlParams, _guiStyle_subTitle, GUILayout.Width(subTitleWidth), GUILayout.Height(subTitleHeight));

#if UNITY_2017_3_OR_NEWER
				_curControlCategory = (apControlParam.CATEGORY)EditorGUILayout.EnumFlagsField(_guiContent_Category.Content, _curControlCategory);
#else
				_curControlCategory = (apControlParam.CATEGORY)EditorGUILayout.EnumMaskPopup(_guiContent_Category.Content, _curControlCategory);
#endif

				EditorGUILayout.Space();
				//1. 컨르롤러를 제어할 수 있도록 하자
					
				if (_controlParams != null)
				{
					for (int i = 0; i < _controlParams.Count; i++)
					{
						if ((int)(_controlParams[i]._category & _curControlCategory) != 0)
						{
							if (GUI_ControlParam(_controlParams[i]))
							{
								isChanged = true;
							}
						}
					}
				}

				GUILayout.Space(30);

				//2. 토글 버튼을 두어서 기본 Inspector 출력 여부를 결정하자.
				string strBaseButton = "Show All Properties";
				if (_showBaseInspector)
				{
					strBaseButton = "Hide Properties";
				}

				if (GUILayout.Button(strBaseButton, GUILayout.Height(20)))
				{
					_showBaseInspector = !_showBaseInspector;
				}

				if (_showBaseInspector)
				{
					base.OnInspectorGUI();
				}


				if (!Application.isPlaying && isChanged)
				{
					//플레이 중이라면 자동으로 업데이트 될 것이다.
					_targetPortrait.UpdateForce();
				}

				if (_targetPortrait != null)
				{	
					if (request_OpenEditor)
					{
						if(_targetPortrait._isOptimizedPortrait)
						{
							RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.Open);
						}
						else
						{
							RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.OpenAndSet);
						}
						//apEditor anyPortraitEditor = apEditor.ShowWindow();
						//if (anyPortraitEditor != null && !_targetPortrait._isOptimizedPortrait)
						//{
						//	anyPortraitEditor.SetPortraitByInspector(_targetPortrait, false);
						//}
					}
					else if (request_QuickBake)
					{
						RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.QuickBake);
						//apEditor anyPortraitEditor = apEditor.ShowWindow();
						//if (anyPortraitEditor != null)
						//{
						//	anyPortraitEditor.SetPortraitByInspector(_targetPortrait, true);

						//	Selection.activeObject = _targetPortrait.gameObject;
						//}
					}
					else if(request_RefreshMeshes)
					{
						_targetPortrait.OnMeshResetInEditor();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("apInspector_Portrait Exception : " + ex);
			}

			//Profiler.EndSample();
		}



		private void Init()
		{
			_curControlCategory = apControlParam.CATEGORY.Head |
									apControlParam.CATEGORY.Body |
									apControlParam.CATEGORY.Face |
									apControlParam.CATEGORY.Hair |
									apControlParam.CATEGORY.Equipment |
									apControlParam.CATEGORY.Force |
									apControlParam.CATEGORY.Etc;

			_showBaseInspector = false;

			//_isFold_BasicSettings = true;
			//_isFold_BasicSettings = true;
			_isFold_RootPortraits = true;
			//_isFold_AnimationSettings = true;
			_isFold_AnimationClips = true;
			//_isFold_ConrolParameters = true;

			_isFold_AnimationEvents = true;
			_isFold_Sockets = true;

			_controlParams = null;
			if (_targetPortrait._controller != null)
			{
				_controlParams = _targetPortrait._controller._controlParams;
			}


			_requestPortrait = null;
			_requestType = REQUEST_TYPE.None;
			_coroutine = null;

#if UNITY_2017_1_OR_NEWER
			_nTimelineTrackSet = (_targetPortrait._timelineTrackSets == null) ? 0 :_targetPortrait._timelineTrackSets.Length;
#endif

			EditorApplication.update -= ExecuteCoroutine;


			if(_socketInfos == null)
			{
				_socketInfos = new List<SocketInfo>();
			}
			_socketInfos.Clear();


			RefreshPrefabStatus();


		}


		private void RefreshPrefabStatus()
		{
			//추가 20.9.14 : 프리팹 여부 체크하기
			_isPrefabAsset = false;
			_isPrefabInstance = false;
			_srcPrefabObject = null;
			_rootGameObjAsPrefabInstance = null;

			//프리팹 에셋이면 아무것도 편집 불가
			_isPrefabAsset = apEditorUtil.IsPrefabAsset(_targetPortrait.gameObject);
			if(_isPrefabAsset)
			{
				return;
			}


			_prefabStatus = apEditorUtil.GetPrefabStatus(_targetPortrait.gameObject);

			switch (_prefabStatus)
			{
				case apEditorUtil.PREFAB_STATUS.NoPrefab:
#if UNITY_2018_3_OR_NEWER
					//이 상태에서 2018.3버전의 프리팹인 경우, 복원 정보가 저장되어 있다면 Disconnected로 변경한다.
					//만약 Disconnected라면 이 정보가 없을 수 있다. (Connected라면 연결되어 있을까?)
					//복원용 정보를 입력하고 Disconnected 상태로 만들자.
					_srcPrefabObject = _targetPortrait._srcPrefabAssetForRestore;
					_rootGameObjAsPrefabInstance = _targetPortrait._rootGameObjectAsPrefabInstanceForRestore;

					if(_srcPrefabObject != null && _rootGameObjAsPrefabInstance != null)
					{
						//둘다 있는 경우에 한해서 Disconnected로 변경
						_isPrefabInstance = true;
						_prefabStatus = apEditorUtil.PREFAB_STATUS.Disconnected;
					}
					else
					{
						//그렇지 않다면 초기화
						_srcPrefabObject = null;
						_rootGameObjAsPrefabInstance = null;
					}
#endif
					break;

				case apEditorUtil.PREFAB_STATUS.Connected:
				case apEditorUtil.PREFAB_STATUS.Disconnected:
					_isPrefabInstance = true;
					_srcPrefabObject = apEditorUtil.GetPrefabObject(_targetPortrait.gameObject);
					_rootGameObjAsPrefabInstance = apEditorUtil.GetRootGameObjectAsPrefabInstance(_targetPortrait.gameObject);
					
					//만약 Disconnected라면 이 정보가 없을 수 있다. (Connected라면 연결되어 있을까?)
					//복원용 정보를 입력하고 Disconnected 상태로 만들자.
					if(_srcPrefabObject == null)
					{
						_srcPrefabObject = _targetPortrait._srcPrefabAssetForRestore;
						_prefabStatus = apEditorUtil.PREFAB_STATUS.Disconnected;
					}
					if(_rootGameObjAsPrefabInstance == null)
					{
						_rootGameObjAsPrefabInstance = _targetPortrait._rootGameObjectAsPrefabInstanceForRestore;
						_prefabStatus = apEditorUtil.PREFAB_STATUS.Disconnected;
					}
					break;

				case apEditorUtil.PREFAB_STATUS.Missing:
					_isPrefabInstance = true;
					break;
			}
		}

		private void FindSockets()
		{
			if(_socketInfos == null)
			{
				_socketInfos = new List<SocketInfo>();
			}
			_socketInfos.Clear();

			if(_targetPortrait == null)
			{
				return;
			}

			int nRootUnits = _targetPortrait._optRootUnitList != null ? _targetPortrait._optRootUnitList.Count : 0;
			apOptRootUnit curRootUnit = null;
			apOptTransform curTF = null;
			apOptBone curBone = null;

			//소켓을 가진 Transform이나 Bone을 찾자
			for (int iRoot = 0; iRoot < nRootUnits; iRoot++)
			{
				curRootUnit = _targetPortrait._optRootUnitList[iRoot];
				if(curRootUnit == null)
				{
					continue;
				}
				int nTransforms = curRootUnit.OptTransforms != null ? curRootUnit.OptTransforms.Count : 0;
				int nBones = curRootUnit.OptBones != null ? curRootUnit.OptBones.Count : 0;

				if (nTransforms > 0)
				{
					for (int iTransform = 0; iTransform < nTransforms; iTransform++)
					{
						curTF = curRootUnit.OptTransforms[iTransform];
						if(curTF._socketTransform != null)
						{
							_socketInfos.Add(new SocketInfo(curTF));
						}
					}
				}

				if (nBones > 0)
				{
					for (int iBone = 0; iBone < nBones; iBone++)
					{
						curBone = curRootUnit.OptBones[iBone];
						if(curBone._socketTransform != null)
						{
							_socketInfos.Add(new SocketInfo(curBone));
						}
					}
				}
			}
			

		}




		private bool GUI_ControlParam(apControlParam controlParam)
		{
			if (controlParam == null)
			{ return false; }

			bool isChanged = false;

			EditorGUILayout.LabelField(controlParam._keyName);

			switch (controlParam._valueType)
			{
				//case apControlParam.TYPE.Bool:
				//	{
				//		bool bPrev = controlParam._bool_Cur;
				//		controlParam._bool_Cur = EditorGUILayout.Toggle(controlParam._bool_Cur);
				//		if(bPrev != controlParam._bool_Cur)
				//		{
				//			isChanged = true;
				//		}
				//	}
				//	break;

				case apControlParam.TYPE.Int:
					{
						int iPrev = controlParam._int_Cur;
						controlParam._int_Cur = EditorGUILayout.IntSlider(controlParam._int_Cur, controlParam._int_Min, controlParam._int_Max);

						if (iPrev != controlParam._int_Cur)
						{
							isChanged = true;
						}
					}
					break;

				case apControlParam.TYPE.Float:
					{
						float fPrev = controlParam._float_Cur;
						controlParam._float_Cur = EditorGUILayout.Slider(controlParam._float_Cur, controlParam._float_Min, controlParam._float_Max);

						if (Mathf.Abs(fPrev - controlParam._float_Cur) > 0.0001f)
						{
							isChanged = true;
						}
					}
					break;

				case apControlParam.TYPE.Vector2:
					{
						Vector2 v2Prev = controlParam._vec2_Cur;
						controlParam._vec2_Cur.x = EditorGUILayout.Slider(controlParam._vec2_Cur.x, controlParam._vec2_Min.x, controlParam._vec2_Max.x);
						controlParam._vec2_Cur.y = EditorGUILayout.Slider(controlParam._vec2_Cur.y, controlParam._vec2_Min.y, controlParam._vec2_Max.y);

						if (Mathf.Abs(v2Prev.x - controlParam._vec2_Cur.x) > 0.0001f ||
							Mathf.Abs(v2Prev.y - controlParam._vec2_Cur.y) > 0.0001f)
						{
							isChanged = true;
						}
					}
					break;

			}

			GUILayout.Space(5);

			return isChanged;
		}


		private apPortrait _requestPortrait = null;
		private enum REQUEST_TYPE
		{
			None,
			Open,
			OpenAndSet,
			QuickBake
		}
		private REQUEST_TYPE _requestType = REQUEST_TYPE.None;
		private IEnumerator _coroutine = null;
		

		private void RequestDelayedOpenEditor(apPortrait portrait, REQUEST_TYPE requestType)
		{
			if(_coroutine != null)
			{
				return;
			}

			_requestPortrait = portrait;
			_requestType = requestType;
			_coroutine = Crt_RequestEditor();

			EditorApplication.update -= ExecuteCoroutine;
			EditorApplication.update += ExecuteCoroutine;
		}

		private void ExecuteCoroutine()
		{
			if(_coroutine == null)
			{
				_requestType = REQUEST_TYPE.None;
				_requestPortrait = null;

				//Debug.Log("ExecuteCoroutine => End");
				EditorApplication.update -= ExecuteCoroutine;
				return;
			}

			//Debug.Log("Update Coroutine");
			bool isResult = _coroutine.MoveNext();
			
			if(!isResult)
			{
				_coroutine = null;
				_requestType = REQUEST_TYPE.None;
				_requestPortrait = null;
				//Debug.Log("ExecuteCoroutine => End");
				EditorApplication.update -= ExecuteCoroutine;
				return;
			}
		}
		private IEnumerator Crt_RequestEditor()
		{
			yield return new WaitForEndOfFrame();
			Selection.activeObject = null;

			yield return new WaitForEndOfFrame();

			if (_requestPortrait != null)
			{	
				try
				{	
					apEditor anyPortraitEditor = apEditor.ShowWindow();
					if (_requestType == REQUEST_TYPE.OpenAndSet)
					{
						anyPortraitEditor.SetPortraitByInspector(_requestPortrait, false);
					}
					else if (_requestType == REQUEST_TYPE.QuickBake)
					{
						anyPortraitEditor.SetPortraitByInspector(_requestPortrait, true);
						Selection.activeObject = _requestPortrait.gameObject;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Open Editor Error : " + ex);
				}
			}
			_requestType = REQUEST_TYPE.None;
			_requestPortrait = null;
		}


		private Texture2D LoadImage(string iconName)
		{
			//이전
			//return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AnyPortrait/Editor/Images/Inspector/" + iconName + ".png");
			//변경 20.4.21 : 경로 변경 설정을 따른다.
			//기본 경로는 "Assets/AnyPortrait/"이므로
			//경로면은 _basePath + "Editor/Images/Inspector/" + iconName + ".png"가 된다.
			return AssetDatabase.LoadAssetAtPath<Texture2D>(_basePath + "Editor/Images/Inspector/" + iconName + ".png");
		}


		//추가 21.8.3 : 애니메이션 이벤트 정보들을 UI에 보여주자

		private struct AnimEventInfo
		{
			public apAnimEvent _animEvent;
			public string _strEventName;
		}

		private AnimEventInfo MakeAnimEventInfo(apAnimEvent animEvent)
		{
			string strAnimEventName = animEvent._eventName;
			
			int nSubParams = animEvent._subParams != null ? animEvent._subParams.Count : 0;
			
			if(nSubParams > 0)
			{
				strAnimEventName += " : ";

				if(nSubParams > 1)
				{
					strAnimEventName += "(object[]) ";
				}

				apAnimEvent.SubParameter subParam = null;
				for (int i = 0; i < nSubParams; i++)
				{
					if(i != 0)
					{
						strAnimEventName += ", ";
					}
					subParam = animEvent._subParams[i];
					switch(subParam._paramType)
					{
						case apAnimEvent.PARAM_TYPE.Bool:		strAnimEventName += "bool";			break;
						case apAnimEvent.PARAM_TYPE.Integer:	strAnimEventName += "int";			break;
						case apAnimEvent.PARAM_TYPE.Float:		strAnimEventName += "float";		break;
						case apAnimEvent.PARAM_TYPE.Vector2:	strAnimEventName += "Vector2";		break;
						case apAnimEvent.PARAM_TYPE.String:		strAnimEventName += "string";		break;
					}
				}
			}

			AnimEventInfo newInfo = new AnimEventInfo();
			newInfo._animEvent = animEvent;
			newInfo._strEventName = strAnimEventName;
			return newInfo;
		}

		//private ANIM_EVENT_SUB_PARAM GetAnimSubParams(apAnimEvent animEvent)
		//{
		//	int nSubParams = animEvent._subParams != null ? animEvent._subParams.Count : 0;
		//	if(nSubParams == 0)
		//	{
		//		return ANIM_EVENT_SUB_PARAM.None;
		//	}
		//	else if(nSubParams == 1)
		//	{
		//		switch (animEvent._subParams[0]._paramType)
		//		{	
		//			case apAnimEvent.PARAM_TYPE.Bool:		return ANIM_EVENT_SUB_PARAM.Bool;
		//			case apAnimEvent.PARAM_TYPE.Integer:	return ANIM_EVENT_SUB_PARAM.Integer;
		//			case apAnimEvent.PARAM_TYPE.Float:		return ANIM_EVENT_SUB_PARAM.Float;
		//			case apAnimEvent.PARAM_TYPE.Vector2:	return ANIM_EVENT_SUB_PARAM.Vector2;
		//			case apAnimEvent.PARAM_TYPE.String:		return ANIM_EVENT_SUB_PARAM.String;
		//		}
		//	}
		//	return ANIM_EVENT_SUB_PARAM.Multiple;
		//}

		//추가 21.8.3 : 애니메이션 이벤트를 복사한다. (스크립트에 붙여넣기 좋게)
		private void CopyAnimEventAsScript(apAnimEvent animEvent)
		{
			string scriptCode = "private void ";
			scriptCode += animEvent._eventName;
			scriptCode += "(";
			
			//파라미터가 있는 경우
			int nSubParams = animEvent._subParams != null ? animEvent._subParams.Count : 0;
			if(nSubParams == 1)
			{
				//한개인 경우
				apAnimEvent.SubParameter subParam = animEvent._subParams[0];
				
				switch (subParam._paramType)
				{	
					case apAnimEvent.PARAM_TYPE.Bool:
						scriptCode += " bool boolValue ";
						break;

					case apAnimEvent.PARAM_TYPE.Integer:
						scriptCode += " int intValue ";
						break;

					case apAnimEvent.PARAM_TYPE.Float:
						scriptCode += " float floatValue ";
						break;

					case apAnimEvent.PARAM_TYPE.Vector2:						
						scriptCode += " Vector2 vecValue ";
						break;

					case apAnimEvent.PARAM_TYPE.String:
						scriptCode += " string strValue ";
						break;
				}
			}
			else if(nSubParams > 1)
			{
				scriptCode += " object[] multipleParams ";
			}
			scriptCode += ")";
			if(nSubParams > 1)
			{
				//파라미터가 여러개인 경우
				scriptCode += "\n{\n";
				apAnimEvent.SubParameter subParam = null;
				for (int i = 0; i < nSubParams; i++)
				{
					subParam = animEvent._subParams[i];
					switch (subParam._paramType)
					{
						case apAnimEvent.PARAM_TYPE.Bool:
							scriptCode += "\tbool boolValue = (bool)multipleParams[" + i + "];\n";
							break;

						case apAnimEvent.PARAM_TYPE.Integer:
							scriptCode += "\tint intValue = (int)multipleParams[" + i + "];\n";
							break;

						case apAnimEvent.PARAM_TYPE.Float:
							scriptCode += "\tfloat floatValue = (float)multipleParams[" + i + "];\n";
							break;

						case apAnimEvent.PARAM_TYPE.Vector2:
							scriptCode += "\tVector2 vecValue = (Vector2)multipleParams[" + i + "];\n";
							break;

						case apAnimEvent.PARAM_TYPE.String:
							scriptCode += "\tstring strValue = (string)multipleParams[" + i + "];\n";
							break;
					}
				}
				scriptCode += "}\n";
			}
			else
			{
				//그외의 경우
				scriptCode += " { }\n";
			}

			EditorGUIUtility.systemCopyBuffer = scriptCode;
			
			Debug.Log("AnyPortrait : The Animation event (" + animEvent._eventName + ") was copied to the clipboard as c# script format.");
		}

		private void CopyAnimEventsAsScript(List<AnimEventInfo> infos)
		{
			int nInfos = infos != null ? infos.Count : 0;
			if(nInfos == 0)
			{
				return;
			}

			string scriptCode = "";
			apAnimEvent animEvent = null;
			for (int iInfo = 0; iInfo < nInfos; iInfo++)
			{
				animEvent = infos[iInfo]._animEvent;

				scriptCode += "private void ";
				scriptCode += animEvent._eventName;
				scriptCode += "(";

				//파라미터가 있는 경우
				int nSubParams = animEvent._subParams != null ? animEvent._subParams.Count : 0;
				if (nSubParams == 1)
				{
					//한개인 경우
					apAnimEvent.SubParameter subParam = animEvent._subParams[0];

					switch (subParam._paramType)
					{
						case apAnimEvent.PARAM_TYPE.Bool:
							scriptCode += " bool boolValue ";
							break;

						case apAnimEvent.PARAM_TYPE.Integer:
							scriptCode += " int intValue ";
							break;

						case apAnimEvent.PARAM_TYPE.Float:
							scriptCode += " float floatValue ";
							break;

						case apAnimEvent.PARAM_TYPE.Vector2:
							scriptCode += " Vector2 vecValue ";
							break;

						case apAnimEvent.PARAM_TYPE.String:
							scriptCode += " string strValue ";
							break;
					}
				}
				else if (nSubParams > 1)
				{
					scriptCode += " object[] multipleParams ";
				}
				scriptCode += ")";
				if (nSubParams > 1)
				{
					//파라미터가 여러개인 경우
					scriptCode += "\n{\n";
					apAnimEvent.SubParameter subParam = null;
					for (int i = 0; i < nSubParams; i++)
					{
						subParam = animEvent._subParams[i];
						switch (subParam._paramType)
						{
							case apAnimEvent.PARAM_TYPE.Bool:
								scriptCode += "\tbool boolValue = (bool)multipleParams[" + i + "];\n";
								break;

							case apAnimEvent.PARAM_TYPE.Integer:
								scriptCode += "\tint intValue = (int)multipleParams[" + i + "];\n";
								break;

							case apAnimEvent.PARAM_TYPE.Float:
								scriptCode += "\tfloat floatValue = (float)multipleParams[" + i + "];\n";
								break;

							case apAnimEvent.PARAM_TYPE.Vector2:
								scriptCode += "\tVector2 vecValue = (Vector2)multipleParams[" + i + "];\n";
								break;

							case apAnimEvent.PARAM_TYPE.String:
								scriptCode += "\tstring strValue = (string)multipleParams[" + i + "];\n";
								break;
						}
					}
					scriptCode += "}\n";
				}
				else
				{
					//그외의 경우
					scriptCode += " { }\n";
				}

				if(iInfo < infos.Count - 1)
				{
					scriptCode += "\n";
				}
			}

			EditorGUIUtility.systemCopyBuffer = scriptCode;
			
			if(nInfos > 1)
			{
				Debug.Log("AnyPortrait : " + infos.Count + " Animation events were copied to the clipboard as c# script format.");
			}
			else
			{
				Debug.Log("AnyPortrait : " + infos.Count + " Animation event was copied to the clipboard as c# script format.");
			}
			
		}



		//추가 21.9.26 : 유니티 이벤트처럼 해당 객체의 함수를 보여준다.
		//타입을 비교해서 처리
		//UnityEvent에 Target Mono를 연결해둘것
		private void ShowMethodsOfUnityEvent(apUnityEvent targetUnityEvent, apUnityEvent.TargetMethodSet methodSet)
		{
			if(targetUnityEvent == null || methodSet == null)
			{
				return;
			}
			GenericMenu newMenu = new GenericMenu();
			if(methodSet._target == null)
			{
				//메소드를 찾을 수 없다.
				newMenu.AddItem(new GUIContent("<No Target>"), false, OnMethodOfUnitEventSelected, null);
			}
			else
			{
				//메소드들을 찾자
				//해당 Mono 뿐만아니라, 그 GameObject의 다른 Monobehaviour도 찾는다.
				//0. 현재의 메소드 이름에 맞는 메소드 Info를 찾자
				bool isAnyMethod = false;

				MethodInfo selectedMethodInfo = GetMethodInfoOfEvent(targetUnityEvent, methodSet);

				//1. 해당 MonoTarget의 메소드들을 찾자.
				List<MethodInfo> methodInfos = GetMethodInfosOfEvent(methodSet._target, targetUnityEvent);
				int nMethodInfos = methodInfos != null ? methodInfos.Count : 0;

				MethodInfo curMethodInfo = null;

				if(nMethodInfos > 0)
				{
					for (int i = 0; i < nMethodInfos; i++)
					{
						curMethodInfo = methodInfos[i];

						//메소드들을 추가한다.
						newMenu.AddItem(new GUIContent(curMethodInfo.Name), MethodInfo.Equals(curMethodInfo, selectedMethodInfo), OnMethodOfUnitEventSelected, new object[] { targetUnityEvent, methodSet, methodSet._target, curMethodInfo});
						isAnyMethod = true;
					}
				}

				//2. 선택된 Mono의 GameObject에 다른 Mono가 있는 경우
				GameObject parentGameObject = methodSet._target.gameObject;
				if(parentGameObject != null)
				{
					MonoBehaviour[] monos = parentGameObject.GetComponents<MonoBehaviour>();
					int nMonos = monos != null ? monos.Length : 0;

					if(nMonos > 1)
					{
						//다른 Mono가 있는 것 같다.
						MonoBehaviour otherMono = null;
						for (int iMono = 0; iMono < nMonos; iMono++)
						{
							otherMono = monos[iMono];
							if(otherMono == methodSet._target)
							{
								//동일하면 패스
								continue;
							}

							methodInfos = GetMethodInfosOfEvent(otherMono, targetUnityEvent);
							nMethodInfos = methodInfos != null ? methodInfos.Count : 0;

							curMethodInfo = null;

							if (nMethodInfos > 0)
							{
								newMenu.AddSeparator("");

								for (int i = 0; i < nMethodInfos; i++)
								{
									curMethodInfo = methodInfos[i];

									//메소드들을 추가한다.
									newMenu.AddItem(new GUIContent(otherMono.GetType().Name + "/" + curMethodInfo.Name), false, OnMethodOfUnitEventSelected, new object[] { targetUnityEvent, methodSet, otherMono, curMethodInfo });
									isAnyMethod = true;
								}
							}
						}
					}
				}

				if(!isAnyMethod)
				{
					//Method가 하나도 없다면
					newMenu.AddItem(new GUIContent("<No Valid Method>"), false, OnMethodOfUnitEventSelected, null);
				}
			}

			newMenu.ShowAsContext();
			Event.current.Use();
		}



		//애니메이션 이벤트 > 콜백 방식인 경우 메소드를 찾아서 연결한다.
		private void OnMethodOfUnitEventSelected(object param)
		{
			if(param == null)
			{
				return;
			}
			if(!(param is object[]))
			{
				return;
			}
			object[] arrParams = param as object[];
			if(arrParams == null || arrParams.Length != 4)
			{
				return;
			}

			apUnityEvent targetEvent = arrParams[0] as apUnityEvent;
			apUnityEvent.TargetMethodSet targetMethodSet = arrParams[1] as apUnityEvent.TargetMethodSet;
			MonoBehaviour monoObj = arrParams[2] as MonoBehaviour;
			MethodInfo methodInfo = arrParams[3] as MethodInfo;

			//하나라도 null이면 종료
			if(targetEvent == null
				|| targetMethodSet == null
				|| monoObj == null
				|| methodInfo == null)
			{
				return;
			}

			Undo.IncrementCurrentGroup();
			Undo.RegisterCompleteObjectUndo(_targetPortrait, "Event Changed");

			targetMethodSet._target = monoObj;
			targetMethodSet._methodName = methodInfo.Name;

			apEditorUtil.SetEditorDirty();
			apEditorUtil.ReleaseGUIFocus();
			if(Event.current != null)
			{
				Event.current.Use();
			}
		}




		private MethodInfo GetMethodInfoOfEvent(apUnityEvent targetUnityEvent, apUnityEvent.TargetMethodSet methodSet)
		{
			if(methodSet._target == null || string.IsNullOrEmpty(methodSet._methodName))
			{
				return null;
			}

			Type type_Mono = methodSet._target.GetType();
			MethodInfo resultMI = type_Mono.GetMethod(methodSet._methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(resultMI == null)
			{
				return null;
			}

			//리턴이 맞아야 한다. (void)
			if(!Type.Equals(resultMI.ReturnType, typeof(void)))
			{
				return null;
			}

			//파라미터 타입이 맞아야 한다.
			ParameterInfo[] paramInfos = resultMI.GetParameters();
			int nParamInfos = paramInfos != null ? paramInfos.Length : 0;

			switch (targetUnityEvent._unityEventType)
			{
				case apUnityEvent.UNITY_EVENT_TYPE.None:
					if(nParamInfos != 0)
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.Bool:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(bool)))
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.Integer:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(int)))
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.Float:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(float)))
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.Vector2:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(Vector2)))
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.String:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(string)))
					{
						return null;
					}
					break;

				case apUnityEvent.UNITY_EVENT_TYPE.MultipleObjects:
					if(nParamInfos != 1 || !Type.Equals(paramInfos[0].ParameterType, typeof(object[])))
					{
						return null;
					}
					break;
			}

			return resultMI;
		}


		private List<MethodInfo> GetMethodInfosOfEvent(MonoBehaviour targetMono, apUnityEvent targetUnityEvent)
		{			
			Type type_Mono = targetMono.GetType();
			MethodInfo[] methods = type_Mono.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			//전체 Method 중에서 해당 유니티 이벤트에 맞는 유니티 이벤트를 찾자
			int nMethods = methods != null ? methods.Length : 0;
			if(nMethods == 0)
			{
				return null;
			}

			List<MethodInfo> resultMIs = new List<MethodInfo>();

			MethodInfo curMethod = null;
			for (int iMethod = 0; iMethod < nMethods; iMethod++)
			{
				curMethod = methods[iMethod];

				//리턴이 맞아야 한다. (void)
				if(!Type.Equals(curMethod.ReturnType, typeof(void)))
				{
					continue;
				}

				//파라미터 타입이 맞아야 한다.
				ParameterInfo[] paramInfos = curMethod.GetParameters();
				int nParamInfos = paramInfos != null ? paramInfos.Length : 0;

				bool isValidParam = false;
				switch (targetUnityEvent._unityEventType)
				{
					case apUnityEvent.UNITY_EVENT_TYPE.None:
						if(nParamInfos == 0)
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.Bool:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(bool)))
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.Integer:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(int)))
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.Float:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(float)))
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.Vector2:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(Vector2)))
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.String:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(string)))
						{
							isValidParam = true;
						}
						break;

					case apUnityEvent.UNITY_EVENT_TYPE.MultipleObjects:
						if(nParamInfos == 1 && Type.Equals(paramInfos[0].ParameterType, typeof(object[])))
						{
							isValidParam = true;
						}
						break;
				}


				if(isValidParam)
				{
					resultMIs.Add(curMethod);
				}
			}

			return resultMIs;
		}

		








		//Event 방식 처리를 위한 배열 UI용 서브 클래스
		public class EventMethodNames
		{
			//연결된 유니티 이벤트
			public apUnityEvent linkedUnityEvent = null;

			//해당 Mono의 
			public MonoBehaviour targetMono = null;
			public MethodInfo[] methodInfos = null;
			public string[] methodNames = null;
		}
	}


	
	

}