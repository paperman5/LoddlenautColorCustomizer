using TMPro;
using UnityEngine;
using I2.Loc;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using iGameAudio.FMODWrapper;
using DG.Tweening;
using Rewired;
using Cinemachine;
using ES3Types;
using UnityEngine.UI.Extensions.ColorPicker;
using UnityEngine.Events;
using System.Collections;

namespace ColorCustomizer
{
    public class ColorCustomizerUI : MonoBehaviour
    {
        private Vector2 canvasSize = new Vector2(1920f, 1080f);
        private Player player;

        private float marginLeft = 30f;
        private float marginTop = 30f;
        private float marginBottom = 30f;
        private float menuWidth = 500f;
        private float pageSelectorMarginBottom = 30f;
        private float entryHeight = 60f;
        private float entrySideMargins = 27.5f;
        private float entryInternalSideMargins = 40f;
        private float entryButtonSideMargin = 20f;
        private float entrySpacing = 11f;
        private float entryPaddingTop = 140f;
        private float entryFontSize = 28f;
        private Vector2 colorButtonSize = new Vector2(75f, 50f);
        private Vector2 resetButtonSize = new Vector2(50f, 50f);
        private Sprite clearSprite;

        private int currentPageIndex = 0;
        private int maxPageIndex = 0;
        private List<ColorPageInfo> currentPageInfo = null;
        private Image[] pageIndicatorDots;
        private Image roundedSquareImage;
        private Color textColor = Color.black;
        private Color resetButtonColor = Color.white;

        private RectTransform pageSelector = null;
        private RectTransform background = null;
        private RectTransform title = null;
        private RectTransform[] pageIndicatorTransforms;
        private List<RectTransform> currentPageEntries = new List<RectTransform>();

        private EventSystem eventSystem;
        private GameObject customizerParent;
        private Canvas customizerCanvas;
        private CanvasGroup customizerCanvasGroup;
        private Sequence menuOpenSequence = null;
        private Vector2 menuOpenSizeDelta;
        private Vector2 menuClosedSizeDelta;
        private Vector3 menuClosedScale = Vector3.one * 0.8f;
        private float menuOpenTime = 0.385f;
        private Ease menuOpenEase = Ease.OutBack;
        private ShippingMenuAudioConfig audioConfig;
        public bool menuIsOpen = false;
        public bool playerMenuIsOpen = false;

        private static CinemachineVirtualCamera menuCamera;
        private Vector3 menuCameraPlayerOffset = new Vector3(1.7f, 0.8f, 5f);
        private Vector3 menuCameraPlayerRotation = new Vector3(8f, 180f, 0f);
        private Vector3 menuCameraShipOffset = Vector3.zero;
        private Vector3 menuCameraShipRotation = new Vector3(0f, 180f, 0f);
        private float menuOpenFov = 50f;
        private float playerTargetAngle = 0f;
        private float playerSmoothVelocity = 0f;
        private float playerSmoothTime = 0.2f;
        private float playerTurnRate = 110.0f;

        private GameObject colorPickerObj;
        private GameObject alphaSlider;
        private RectTransform colorPicker;
        private RectTransform colorPickerRoot;
        private ColorPickerControl pickerControl;
        private CanvasGroup pickerCanvasGroup;
        private Sequence pickerOpenSequence;
        private UnityAction<Color> colorEventListener;
        private float pickerTargetPos = 0f;
        private ColorMenuEntry selectedEntry;
        private bool pickerIsOpen = false;


        #region Setting Page Data

        private List<ColorPageInfo> playerPageInfo = new List<ColorPageInfo>
        {
            new ColorPageInfo
            {
                pageTitle = "Suit",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.helmetMain,        "Helmet"        },
                    { PlayerKeyNames.helmetAntennaStem, "Antenna Stem"  },
                    { PlayerKeyNames.helmetWindowRing,  "Window Ring"   },
                    { PlayerKeyNames.suitCollar,        "Collar"        },
                    { PlayerKeyNames.suitMain,          "Main Suit"     },
                    { PlayerKeyNames.suitArmsLegs,      "Arms & Legs"   },
                    { PlayerKeyNames.suitCuffs,         "Cuffs"         },
                    { PlayerKeyNames.suitLapel1,        "Lapel 1"       },
                    { PlayerKeyNames.suitLapel2,        "Lapel 2"       },
                    { PlayerKeyNames.suitPocketBottom,  "Pocket Bottom" },
                    { PlayerKeyNames.suitPocketTop,     "Pocket Top"    },
                },
                canOpenPage = () => true,
            },
            new ColorPageInfo
            {
                pageTitle = "Depth Suit",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.helmetDepthMain,       "Helmet"        },
                    { PlayerKeyNames.helmetDepthAntennaStem,"Antenna Stem"  },
                    { PlayerKeyNames.helmetDepthWindowRing, "Window Ring"   },
                    { PlayerKeyNames.suitDepthCollar,       "Collar"        },
                    { PlayerKeyNames.suitDepthMain,         "Main Suit"     },
                    { PlayerKeyNames.suitDepthArmsLegs,     "Arms & Legs"   },
                    { PlayerKeyNames.suitDepthCuffs,        "Cuffs"         },
                    { PlayerKeyNames.suitDepthLapel1,       "Lapel 1"       },
                    { PlayerKeyNames.suitDepthLapel2,       "Lapel 2"       },
                    { PlayerKeyNames.suitDepthPocketBottom, "Pocket Bottom" },
                    { PlayerKeyNames.suitDepthPocketTop,    "Pocket Top"    },
                },
                canOpenPage = () => EngineHub.Upgrades.depthModuleIsUnlocked,
            },
            new ColorPageInfo
            {
                pageTitle = "Suit Common",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.suitAntennaOrbInner,   "Antenna Inner" },
                    { PlayerKeyNames.suitAntennaOrbOuter,   "Antenna Outer" },
                    { PlayerKeyNames.helmetWindow,          "Helmet Window" },
                    { PlayerKeyNames.helmetCheeks,          "Helmet Lights" },
                },
                canOpenPage = () => true,
            },
            new ColorPageInfo
            {
                pageTitle = "Jetpack (1)",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.jetpackTop,                        "Top"              },
                    { PlayerKeyNames.jetpackTopBooster1,                "Top Booster 1"    },
                    { PlayerKeyNames.jetpackTopBooster2,                "Top Booster 2"    },
                    { PlayerKeyNames.jetpackBottom,                     "Bottom"           },
                    { PlayerKeyNames.jetpackBottomBooster1,             "Bottom Booster 1" },
                    { PlayerKeyNames.jetpackBottomBooster2,             "Bottom Booster 2" },
                    { PlayerKeyNames.jetpackPropellerConnector,         "Propeller Ring 1" },
                    { PlayerKeyNames.jetpackPropellerConnectorInside,   "Propeller Ring 2" },
                    { PlayerKeyNames.jetpackPipeConnector,              "Pipe Ring"        },
                    { PlayerKeyNames.jetpackPipe,                       "Pipe"             },
                    { PlayerKeyNames.jetpackBoostMeterOverdrive,        "Boost Overdrive"  },
                },
                canOpenPage = () => true,
            },
            new ColorPageInfo
            {
                pageTitle = "Jetpack (2)",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.jetpackPropellerBlades,    "Propeller Blades"  },
                    { PlayerKeyNames.jetpackPropellerShaft,     "Propeller Shaft"   },
                    { PlayerKeyNames.jetpackPropellerHub,       "Propeller Hub"     },
                    { PlayerKeyNames.jetpackArrows,             "Arrows On"         },
                    { PlayerKeyNames.jetpackArrowsOff,          "Arrows Off"        },
                    { PlayerKeyNames.jetpackBoostMeter ,        "Boost Meter"       },
                    { PlayerKeyNames.jetpackBoostMeterUpgrade,  "Upgraded Boost"    },
                    { PlayerKeyNames.jetpackBoostMeterOff,      "Boost Depleted"    },
                    { PlayerKeyNames.jetpackOxygenMeter,        "Oxygen Meter"      },
                    { PlayerKeyNames.jetpackOxygenMeterLow,     "Oxygen Low"        },
                    { PlayerKeyNames.jetpackOxygenMeterOff,     "Oxygen Depleted"   },
                },
                canOpenPage = () => true,
            },
            new ColorPageInfo
            {
                pageTitle = "Oxygen Upgrades",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.jetpackOxygenUpgradeLight, "Light" },
                    { PlayerKeyNames.jetpackOxygenUpgradeGlow,  "Glow"  },
                    { PlayerKeyNames.jetpackOxygenUpgradeCap,   "Cap"   },
                    { PlayerKeyNames.jetpackOxygenUpgradeRing1, "Ring 1"},
                    { PlayerKeyNames.jetpackOxygenUpgradeRing2, "Ring 2"},
                },
                canOpenPage = () => EngineHub.Upgrades.oxygenUpgradeCount > 0,
            },
            new ColorPageInfo
            {
                pageTitle = "Bubble Gun",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.gunMain1,  "Main 1" },
                    { PlayerKeyNames.gunMain2,  "Main 2" },
                    { PlayerKeyNames.gunHandle, "Handle" },
                    { PlayerKeyNames.gunTip1,   "Tip 1" },
                    { PlayerKeyNames.gunTip2,   "Tip 2" },
                    { PlayerKeyNames.gunGlass,  "Glass" },
                    { PlayerKeyNames.gunCoreInactive,   "Core Inactive" },
                    // active color should also change flicker color
                    { PlayerKeyNames.gunCoreActive,     "Core Active" },
                    { PlayerKeyNames.gunLaser,  "Laser" },
                },
                canOpenPage = () => true,
            },
            new ColorPageInfo
            {
                pageTitle = "Blaster Upgrade",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.gunBlasterBarsInactive,  "Blaster Bars" },
                    // active color should also change inactive color
                    //{ PlayerKeyNames.gunBlasterBarsActive,  "Bars Active" },
                    { PlayerKeyNames.gunBlasterRod1, "Blaster Rod 1" },
                    { PlayerKeyNames.gunBlasterRod2, "Blaster Rod 2" },
                    { PlayerKeyNames.gunBlasterRod3, "Blaster Rod 3" },
                    { PlayerKeyNames.gunBlasterBall, "Blaster Ball" },
                },
                canOpenPage = () => EngineHub.Upgrades.blasterModeIsUnlocked,
            },
            new ColorPageInfo
            {
                pageTitle = "Efficiency Upgrade",
                settings = new Dictionary<string, string>
                {
                    { PlayerKeyNames.gunCoreActiveUpgraded,  "Efficiency Core" },
                    { PlayerKeyNames.gunLaserUpgraded,  "Efficiency Laser" },
                },
                canOpenPage = () => EngineHub.Upgrades.laserEfficiencyUpgradeIsUnlocked,
            },
            new ColorPageInfo
            {
                pageTitle = "Scrap Vac",
                settings = new Dictionary<string, string>(),
                canOpenPage = () => EngineHub.PlayerEquipment.equipmentUnlockStates[1],
            },
            new ColorPageInfo
            {
                pageTitle = "Puddle Scrubber",
                settings = new Dictionary<string, string>(),
                canOpenPage = () => EngineHub.PlayerEquipment.equipmentUnlockStates[2],
            }
        };
        private List<ColorPageInfo> shipPageInfo = new List<ColorPageInfo>();

        #endregion

        public void Awake()
        {
            player = ReInput.players.GetPlayer(0);
            CustomizerMod.customizerUI = this;
            eventSystem = EventSystem.current;
            customizerParent = transform.parent.gameObject;
            customizerCanvas = customizerParent.GetComponent<Canvas>();
            customizerCanvasGroup = customizerParent.GetComponent<CanvasGroup>();
            customizerCanvasGroup.interactable = false;
            customizerCanvasGroup.alpha = 0f;

            StartCoroutine(DelayedApplyAllColors());
        }

        public void Start()
        {
            CreatePrototypes();
            InitializeColorCustomizerUI();

            var camObj = new GameObject("Customizer Menu Camera");
            menuCamera = camObj.AddComponent<CinemachineVirtualCamera>();
            menuCamera.Priority = -10;
            menuCamera.enabled = false;
            menuCamera.m_Lens.FieldOfView = menuOpenFov;

            camObj.transform.position = menuCameraPlayerOffset;
            camObj.transform.eulerAngles = menuCameraPlayerRotation;
            camObj.transform.SetParent(null);

            background.gameObject.SetActive(false);
            pickerCanvasGroup.alpha = 0f;
            pickerCanvasGroup.interactable = false;

            InputModifier.CreateAndApplyCustomMaps();
        }

        public void Update()
        {
            if (menuIsOpen && Input.GetMouseButtonUp(0))
            {
                eventSystem.SetSelectedGameObject(null);
            }

            if (player.GetButtonDown(KeybindingNames.openColorMenu))
            {
                if (EngineHub.InventoryUI.inventoryIsOpen)
                {
                    EngineHub.InventoryUI.ToggleInventoryUI();
                }
                OpenPlayerCustomizeMenu();
            }

            if (player.GetButtonDown("Structure Exit") && menuIsOpen)
            {
                CloseMenu();
            }

            if (playerMenuIsOpen)
            {
                if (player.GetButton(KeybindingNames.rotateModelLeft))
                {
                    playerTargetAngle -= playerTurnRate * Time.deltaTime;
                }
                if (player.GetButton(KeybindingNames.rotateModelRight))
                {
                    playerTargetAngle += playerTurnRate * Time.deltaTime;
                }
                playerTargetAngle %= 360f;

                Transform t = EngineHub.PlayerTransforms.controllerRootTransform;
                t.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(t.eulerAngles.y, playerTargetAngle, ref playerSmoothVelocity, playerSmoothTime);

                menuCamera.transform.position = new Vector3(menuCamera.transform.position.x, 
                        EngineHub.PlayerTransforms.controllerRootTransform.position.y + menuCameraPlayerOffset.y, 
                        menuCamera.transform.position.z);
            }
        }

        private void OpenMenuBasic()
        {
            eventSystem = EventSystem.current;
            menuIsOpen = true;
            customizerCanvas.enabled = true;
            customizerCanvasGroup.alpha = 1f;
            background.gameObject.SetActive(true);
            EngineHub.UIManager.FadeAllCanvasesExcept(0f, customizerCanvasGroup, -1f, false, false);
            customizerCanvasGroup.interactable = true;
            customizerCanvasGroup.blocksRaycasts = true;
            EngineHub.HQMessageManager.PauseMessages();
            // TODO: Input switching handling, see ShippingMenu

            Sequence sequence = menuOpenSequence;
            if (sequence != null)
            {
                sequence.Kill(false);
            }
            menuOpenSequence = DOTween.Sequence();
            menuOpenSequence.Append(background.DOSizeDelta(menuOpenSizeDelta, menuOpenTime, false).SetEase(menuOpenEase));
            menuOpenSequence.Join(background.DOScale(Vector3.one, menuOpenTime).SetEase(menuOpenEase));
            menuOpenSequence.Join(customizerCanvasGroup.DOFade(1f, menuOpenTime).SetEase(menuOpenEase));
            //this.menuOpenSequence.Join(this.controlsText.DOFade(1f, this.controlsTextFadeTime).SetEase(this.controlsTextFadeEase).SetDelay(this.controlsTextFadeInDelay));
            AudioSystem.PlaySound(audioConfig.OpenSound, default(Vector3));
            EngineHub.CentralGameMenu.DisableMapSegmentCanvasRaycasts();
            EngineHub.InputDispatcher.SwitchToStructureContext();
        }

        public void OpenPlayerCustomizeMenu()
        {
            if (menuIsOpen)
                return;
            // Set up the page selector & layout
            OpenMenuBasic();
            currentPageInfo = playerPageInfo;
            LayoutPageSelector(playerPageInfo);
            LayoutPageEntries(0, playerPageInfo, true);
            TurnOnPlayerFocusCamera();
            EngineHub.PlayerAnimator.CalculateAndSetHeadTilt(); // Stop looking at loddles
            playerMenuIsOpen = true;
        }

        public void CloseMenu()
        {
            if (!menuIsOpen)
                return;

            EngineHub.UIManager.FadeAllCanvasesExcept(1f, customizerCanvasGroup, -1f, false, false);
            customizerCanvasGroup.interactable = false;
            customizerCanvasGroup.blocksRaycasts = false;
            EngineHub.HQMessageManager.ResumeMessages();
            Sequence sequence = menuOpenSequence;
            if (sequence != null)
            {
                sequence.Kill(false);
            }
            menuOpenSequence = DOTween.Sequence();
            menuOpenSequence.SetUpdate(true);
            menuOpenSequence.Append(background.DOSizeDelta(menuClosedSizeDelta, menuOpenTime, false).SetEase(menuOpenEase));
            menuOpenSequence.Join(background.DOScale(menuClosedScale, menuOpenTime).SetEase(menuOpenEase));
            menuOpenSequence.Join(customizerCanvasGroup.DOFade(0f, menuOpenTime).SetEase(menuOpenEase));
            //this.menuOpenSequence.Join(this.controlsText.DOFade(0f, this.controlsTextFadeTime).SetEase(this.controlsTextFadeEase));
            menuOpenSequence.AppendCallback(new TweenCallback(DisableMenu));
            AudioSystem.PlaySound(audioConfig.CloseSound, default(Vector3));
            EngineHub.CentralGameMenu.EnableMapSegmentCanvasRaycasts();
            EngineHub.InputDispatcher.SwitchToPreviousInputContext();

            CloseColorPicker();
            TurnOffMenuCamera();
            menuIsOpen = false;
            playerMenuIsOpen = false;
        }

        private void DisableMenu()
        {
            background.gameObject.SetActive(false);
        }

        private void InitializeColorCustomizerUI()
        {
            // Set up this RectTransform layout
            RectTransform customizerTransform = GetComponent<RectTransform>();
            customizerTransform.anchorMin = Vector2.zero;
            customizerTransform.anchorMax = Vector2.zero;
            customizerTransform.pivot = Vector2.zero;
            customizerTransform.anchoredPosition = Vector2.zero;

            // Create menu background
            GameObject background = Instantiate(CustomizerMod.uiBackgroundPrototype);
            background.SetActive(true);
            background.transform.SetParent(transform, false);

            RectTransform rt = background.GetComponent<RectTransform>();
            rt.pivot = Vector2.one * 0.5f;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(marginLeft + menuWidth / 2f, canvasSize.y / 2f);
            rt.sizeDelta = new Vector2(menuWidth, canvasSize.y - marginTop - marginBottom);
            menuOpenSizeDelta = rt.sizeDelta;
            menuClosedSizeDelta = new Vector2(menuOpenSizeDelta.x * 0.9f, menuOpenSizeDelta.y * 0.8f);
            this.background = background.GetComponent<RectTransform>();

            // Create title
            GameObject title = Instantiate(CustomizerMod.uiTitlePrototype);
            title.SetActive(true);
            title.GetComponentInChildren<TextMeshProUGUI>().text = "Colors";
            title.transform.SetParent(background.transform, false);
            this.title = title.GetComponent<RectTransform>();

            // Create page controls (this will not set the layout correctly, only initializes)
            GameObject pageSelector = Instantiate(CustomizerMod.uiPageSelectorPrototype);
            pageSelector.SetActive(true);
            int maxPageSelectors = Mathf.Max(playerPageInfo.Count, shipPageInfo.Count);
            // Instantiate the page indicator dots
            GameObject dotPrototype = Instantiate(pageSelector.transform.GetChild(0).gameObject);
            dotPrototype.SetActive(true);
            var dotImageList = new List<Image>();
            var dotTransformList = new List<RectTransform>();
            DestroyImmediate(pageSelector.transform.GetChild(0).gameObject);
            for (int i = 0; i < maxPageSelectors; i++)
            {
                GameObject dot = Instantiate(dotPrototype);
                dot.SetActive(true);
                dot.transform.SetParent(pageSelector.transform, false);
                dot.transform.SetSiblingIndex(i);
                dot.name = "PageCounter" + i.ToString();
                dotImageList.Add(dot.GetComponent<Image>());
                dotTransformList.Add(dot.GetComponent<RectTransform>());
            }
            Destroy(dotPrototype);
            pageIndicatorDots = dotImageList.ToArray();
            pageIndicatorTransforms = dotTransformList.ToArray();
            maxPageIndex = maxPageSelectors - 1;

            // Hook up page control events
            BetterButton leftButton = pageSelector.transform.Find("LeftPageButton").GetComponent<BetterButton>();
            leftButton.onClick.AddListener(new UnityAction(ShowPreviousPage));
            leftButton.onClick.AddListener(new UnityAction(CloseColorPicker));
            BetterButton rightButton = pageSelector.transform.Find("RightPageButton").GetComponent<BetterButton>();
            rightButton.onClick.AddListener(new UnityAction(ShowNextPage));
            rightButton.onClick.AddListener(new UnityAction(CloseColorPicker));
            pageSelector.transform.SetParent(background.transform, false);
            this.pageSelector = pageSelector.GetComponent<RectTransform>();

            // Color Picker
            colorPicker.GetComponent<Image>().material = CustomizerMod.uiBackgroundPrototype.GetComponent<Image>().material;
            colorPickerRoot = colorPickerObj.GetComponent<RectTransform>();
            pickerCanvasGroup = colorPickerObj.GetComponent<CanvasGroup>();
            colorPickerRoot.Find("Arrow").GetComponent<Image>().material = CustomizerMod.uiBackgroundPrototype.GetComponent<Image>().material;
            colorPickerRoot.anchoredPosition = new Vector2(marginLeft * 2f + menuWidth, 0f);
            colorPickerObj.SetActive(true);
            pickerCanvasGroup.alpha = 0f;
            pickerCanvasGroup.interactable = false;

            this.background.sizeDelta = menuClosedSizeDelta;
            this.background.localScale = menuClosedScale;
        }

        private void LayoutPageSelector(List<ColorPageInfo> pages)
        {
            float spacing = EngineHub.ShippingMenu.counterSpacing;
            float padding = EngineHub.ShippingMenu.counterBackgroundPadding;

            // Disable all dots and count how many should be shown
            int numIndicators = 0;
            for (int i = 0; i < pageIndicatorDots.Length; i++)
            {
                pageIndicatorDots[i].enabled = false;
                if (i < pages.Count && pages[i].canOpenPage())
                {
                    numIndicators++;
                }
            }

            // Set the size according to how many pages can be opened and place the dots
            pageSelector.sizeDelta = new Vector2(Mathf.Max(numIndicators * spacing + padding, spacing + padding), pageSelector.sizeDelta.y);
            int currentDot = 0;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].canOpenPage())
                {
                    pageIndicatorDots[i].enabled = true;
                    pageIndicatorTransforms[i].anchoredPosition = new Vector2(currentDot * spacing + padding, 0f);
                    currentDot++;
                }
            }

            // Place the overall page selector on the background
            pageSelector.anchorMin = new Vector2(0.5f, 0f);
            pageSelector.anchorMax = new Vector2(0.5f, 0f);
            pageSelector.anchoredPosition = new Vector2(0f, pageSelector.sizeDelta.y / 2f + pageSelectorMarginBottom);
        }

        private void LayoutPageEntries(int pageIndex, List<ColorPageInfo> pages, bool forceUpdate = false)
        {
            if (pageIndex < 0 || pageIndex >= pages.Count || (pageIndex == currentPageIndex && !forceUpdate))
            {
                return;
            }
            foreach (var t in currentPageEntries)
            {
                Destroy(t.gameObject);
            }
            currentPageEntries.Clear();

            title.GetComponentInChildren<TextMeshProUGUI>().text = pages[pageIndex].pageTitle;

            var settingKeys = pages[pageIndex].settings.Keys.ToArray();
            var settingNames = pages[pageIndex].settings.Values.ToArray();
            var colorButtons = new List<BetterButton>();
            var resetButtons = new List<BetterButton>();
            for (int entryIndex = 0; entryIndex < pages[pageIndex].settings.Count; entryIndex++)
            {
                GameObject entry = Instantiate(CustomizerMod.colorMenuEntryPrototype);
                entry.SetActive(true);
                var rt = entry.GetComponent<RectTransform>();
                var menuEntry = entry.GetComponent<ColorMenuEntry>();
                menuEntry.Initialize(settingKeys[entryIndex], settingNames[entryIndex]);
                rt.SetParent(background, false);
                rt.anchoredPosition = new Vector2(0f, -(entryPaddingTop + entryIndex * (entryHeight + entrySpacing)));
                currentPageEntries.Add(rt);
                BetterButton colorButton = rt.Find("ColorButton").GetComponent<BetterButton>();
                BetterButton resetButton = rt.Find("ResetButton").GetComponent<BetterButton>();
                colorButtons.Add(colorButton);
                resetButtons.Add(resetButton);

                colorButton.onClick.AddListener(new UnityAction(() => OnColorButtonPressed(menuEntry)));
            }

            // Set up navigation & events
            for (int entryIndex = 0; entryIndex < currentPageEntries.Count; entryIndex++)
            {
                Navigation colNav = new Navigation();
                Navigation resNav = new Navigation();
                if (entryIndex > 0)
                {
                    colNav.selectOnUp = colorButtons[entryIndex - 1];
                    resNav.selectOnUp = resetButtons[entryIndex - 1];
                }
                if (entryIndex < currentPageEntries.Count - 2)
                {
                    colNav.selectOnDown = colorButtons[entryIndex + 1];
                    resNav.selectOnDown = colorButtons[entryIndex + 1];
                }
                colNav.selectOnRight = resetButtons[entryIndex];
                resNav.selectOnLeft = colorButtons[entryIndex];

                colorButtons[entryIndex].navigation = colNav;
                resetButtons[entryIndex].navigation = resNav;

                var menuEntry = currentPageEntries[entryIndex].GetComponent<ColorMenuEntry>();
                resetButtons[entryIndex].onClick.AddListener(new UnityAction(() => OnResetButtonPressed(menuEntry)));
            }

            currentPageIndex = pageIndex;
        }

        private void ShowNextPage()
        {
            int nextPageIndex = Mathf.Min(currentPageIndex + 1, currentPageInfo.Count - 1);
            while (!currentPageInfo[nextPageIndex].canOpenPage() && nextPageIndex < currentPageInfo.Count - 1)
            {
                nextPageIndex++;
            }
            if (!currentPageInfo[nextPageIndex].canOpenPage())
            {
                nextPageIndex = currentPageIndex;
            }
            CustomizerPlugin.Logger.LogInfo($"Selecting page {nextPageIndex}");
            LayoutPageEntries(nextPageIndex, currentPageInfo);
        }

        private void ShowPreviousPage()
        {
            int prevPageIndex = Mathf.Max(0, currentPageIndex - 1);
            while (!currentPageInfo[prevPageIndex].canOpenPage() && prevPageIndex > 0)
            {
                prevPageIndex--;
            }
            if (!currentPageInfo[prevPageIndex].canOpenPage())
            {
                prevPageIndex = currentPageIndex;
            }
            CustomizerPlugin.Logger.LogInfo($"Selecting page {prevPageIndex}");
            LayoutPageEntries(prevPageIndex, currentPageInfo);
        }

        private void CreatePrototypes()
        {
            roundedSquareImage = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/ShippingMenu/ShippingMenuParent/ShippingMenuBackground/UpgradeEntries/ShippingMenuEntry_Upgrade1").GetComponent<Image>();
            textColor = roundedSquareImage.transform.Find("EntryTitleText").GetComponent<TextMeshProUGUI>().color;
            resetButtonColor = roundedSquareImage.transform.Find("EntryOrderButton").GetComponent<Image>().color;
            clearSprite = Sprite.Create((Texture2D)CustomizerPlugin.customizerAssets.LoadAsset("reset_icon"), new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 200f);
            audioConfig = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/ShippingMenu").GetComponent<ShippingMenu>().audioConfig;

            if (CustomizerMod.uiBackgroundPrototype == null)
                CustomizerMod.uiBackgroundPrototype = CreateUIBackgroundPrototype();
            if (CustomizerMod.uiTitlePrototype == null)
                CustomizerMod.uiTitlePrototype = CreateUITitlePrototype();
            if (CustomizerMod.uiButtonPrototype == null)
                CustomizerMod.uiButtonPrototype = CreateUIButtonPrototype();
            if (CustomizerMod.uiPageSelectorPrototype == null)
                CustomizerMod.uiPageSelectorPrototype = CreateUIPageSelectorPrototype();
            if (CustomizerMod.uiLeftArrowButtonPrototype == null)
                CustomizerMod.uiLeftArrowButtonPrototype = CreateUILeftArrowButtonPrototype();
            if (CustomizerMod.uiRightArrowButtonPrototype == null)
                CustomizerMod.uiRightArrowButtonPrototype = CreateUIRightArrowButtonPrototype();
            if (CustomizerMod.colorMenuEntryPrototype == null)
                CustomizerMod.colorMenuEntryPrototype = CreateColorMenuEntryPrototype();
            if (colorPickerObj == null)
            {
                GameObject colorPickerPrefab = CustomizerPlugin.customizerAssets.LoadAsset<GameObject>("PickerRoot");
                colorPickerObj = Instantiate(colorPickerPrefab);
                colorPicker = colorPickerObj.transform.GetChild(0).GetComponent<RectTransform>();
                pickerControl = colorPicker.GetComponent<ColorPickerControl>();
                colorPickerObj.transform.SetParent(transform, false);
                colorPickerObj.SetActive(false);
                foreach (var slider in colorPickerObj.GetComponentsInChildren<ColorSlider>())
                {
                    GameObject n = slider.transform.parent.gameObject;
                    if (n.name == "A")
                    {
                        alphaSlider = n;
                        break;
                    }
                }
            }
        }

        private GameObject CreateUIBackgroundPrototype()
        {
            GameObject existingBackground = GameObject.Find("/PlayerRoot/UI Manager/HUD Canvas/LoddleInfoMenuParent/LoddleInfoMenuBackground");
            if (existingBackground == null)
            {
                return null;
            }
            
            GameObject prototype = Instantiate(existingBackground);
            prototype.name = "MenuBackground";
            while (prototype.transform.childCount > 0)
            {
                DestroyImmediate(prototype.transform.GetChild(0).gameObject);
            }
            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Menu background prototype created");

            return prototype;
        }

        private GameObject CreateUITitlePrototype()
        {
            GameObject existingTitle = GameObject.Find("/PlayerRoot/UI Manager/HUD Canvas/LoddleInfoMenuParent/LoddleInfoMenuBackground/Loddle Name Background");
            if (existingTitle == null)
            {
                return null;
            }

            GameObject prototype = Instantiate(existingTitle);
            prototype.name = "Title";
            TextMeshProUGUI titleText = prototype.GetComponentInChildren<TextMeshProUGUI>();
            titleText.gameObject.name = "TitleText";
            titleText.text = "";
            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Menu title prototype created");

            return prototype;
        }

        private GameObject CreateUIButtonPrototype()
        {
            GameObject existingButton = GameObject.Find("/PlayerRoot/UI Manager/HUD Canvas/LoddleInfoMenuParent/LoddleInfoMenuBackground/LoddleInfoMenuMainButtonsParent/Loddle Status Button");
            if (existingButton == null)
            {
                return null;
            }

            GameObject prototype = Instantiate(existingButton);
            prototype.name = "Button";

            // Remove the 'Localize' component
            DestroyImmediate(prototype.GetComponentInChildren<Localize>());

            TextMeshProUGUI buttonText = prototype.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.gameObject.name = "ButtonText";
            buttonText.text = "";

            BetterButton button = prototype.GetComponent<BetterButton>();
            RemoveButtonPersistentListeners(button);
            
            // Remove navigation
            button.navigation = new UnityEngine.UI.Navigation();

            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Menu button prototype created");

            return prototype;
        }

        private GameObject CreateUIPageSelectorPrototype()
        {
            GameObject existing = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/ShippingMenu/ShippingMenuParent/ShippingMenuBackground/UpgradePageCounters");
            if (existing == null)
            {
                return null;
            }

            GameObject prototype = Instantiate(existing);
            prototype.name = "PageSelector";

            BetterButton leftButton = prototype.transform.Find("LeftPageButton").GetComponent<BetterButton>();
            BetterButton rightButton = prototype.transform.Find("RightPageButton").GetComponent<BetterButton>();
            RemoveButtonPersistentListeners(leftButton);
            RemoveButtonPersistentListeners(rightButton);
            leftButton.navigation = new UnityEngine.UI.Navigation
            {
                selectOnRight = leftButton.navigation.selectOnRight
            };
            rightButton.navigation = new UnityEngine.UI.Navigation
            {
                selectOnLeft = rightButton.navigation.selectOnLeft
            };
            // Get a page counter dot, save a copy, delete all dots, then add one back in
            GameObject pageCounterPrototype = null;
            for (int i = 0; i < prototype.transform.childCount; i++)
            {
                Transform child = prototype.transform.GetChild(i);
                if (child.name.Contains("PageCounter"))
                {
                    pageCounterPrototype = Instantiate(child.gameObject);
                    break;
                }
            }
            TryAgain:
            // loop through all children and delete all page counters, but don't delete other children
            for (int i = 0; i < prototype.transform.childCount; i++)
            {
                Transform child = prototype.transform.GetChild(i);
                if (child.name.Contains("PageCounter"))
                {
                    DestroyImmediate(child.gameObject);
                    goto TryAgain;
                }
            }
            GameObject pc = Instantiate(pageCounterPrototype);
            pc.name = "PageCounter0";
            pc.transform.SetParent(prototype.transform, false);
            pc.transform.SetSiblingIndex(0);
            pc.SetActive(true);

            // Remove the CanvasGroup
            DestroyImmediate(prototype.GetComponent<CanvasGroup>());

            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Page selector prototype created");

            return prototype;
        }

        private GameObject CreateUILeftArrowButtonPrototype()
        {
            GameObject existing = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/ShippingMenu/ShippingMenuParent/ShippingMenuBackground/UpgradePageCounters/LeftPageButton");
            if (existing == null)
            {
                return null;
            }

            GameObject prototype = Instantiate(existing);
            prototype.name = "LeftArrowButton";

            BetterButton button = prototype.GetComponent<BetterButton>();
            RemoveButtonPersistentListeners(button);
            button.navigation = new UnityEngine.UI.Navigation();

            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Left arrow button prototype created");

            return prototype;
        }

        private GameObject CreateUIRightArrowButtonPrototype()
        {
            GameObject existing = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/ShippingMenu/ShippingMenuParent/ShippingMenuBackground/UpgradePageCounters/RightPageButton");
            if (existing == null)
            {
                return null;
            }

            GameObject prototype = Instantiate(existing);
            prototype.name = "RightArrowButton";

            BetterButton button = prototype.GetComponent<BetterButton>();
            RemoveButtonPersistentListeners(button);
            button.navigation = new UnityEngine.UI.Navigation();

            prototype.SetActive(false);
            prototype.transform.localScale = Vector3.one;
            prototype.transform.SetParent(null);

            CustomizerPlugin.Logger.LogInfo("Right arrow button prototype created");

            return prototype;
        }

        private GameObject CreateColorMenuEntryPrototype()
        {
            if (CustomizerMod.uiButtonPrototype == null)
            {
                CreateUIButtonPrototype();
            }

            //Sprite betterButtonSprite = CustomizerPlugin.customizerAssets.LoadAsset<Sprite>("color_button");
            Material checkerboardMaterial = CustomizerPlugin.customizerAssets.LoadAsset<Material>("CheckerboardTransparency");
            GameObject betterSpriteObj = GameObject.Find("/PlayerRoot/UI Manager/Structure Menu Canvas/CentralGameMenu/CentralGameMenuParent/CentralGameMenuBackground/ObjectivesTabContents/ObjectiveMenuCard/ObjectiveHeaderBG");
            Sprite betterButtonSprite = betterSpriteObj.GetComponent<Image>().sprite;

            // Create a "prefab" of sorts, from code
            GameObject prototype = new GameObject("ColorMenuEntry");
            RectTransform rt = prototype.AddComponent<RectTransform>();
            Image img = CustomizerMod.CopyComponent(roundedSquareImage, prototype);
            ColorMenuEntry menuEntry = prototype.AddComponent<ColorMenuEntry>();

            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.sizeDelta = new Vector2(-entrySideMargins * 2, entryHeight);

            // Layout & styling
            GameObject entryText = new GameObject("EntryText");
            RectTransform rt2 = entryText.AddComponent<RectTransform>();
            TextMeshProUGUI tmp = entryText.AddComponent<TextMeshProUGUI>();
            rt2.anchorMin = Vector2.zero;
            rt2.anchorMax = Vector2.one;
            rt2.sizeDelta = new Vector2(-entryInternalSideMargins * 2, 0f);
            tmp.verticalAlignment = VerticalAlignmentOptions.Capline;
            tmp.color = textColor;
            tmp.fontSizeMin = entryFontSize;
            tmp.fontSizeMax = entryFontSize;
            tmp.fontSize = entryFontSize;
            entryText.transform.SetParent(prototype.transform, false);

            // Create color & reset buttons
            GameObject colorButtonObj = Instantiate(CustomizerMod.uiButtonPrototype);
            colorButtonObj.GetComponent<Image>().material = checkerboardMaterial;
            colorButtonObj.GetComponent<Image>().sprite = betterButtonSprite;
            colorButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false); // disable text
            colorButtonObj.SetActive(true);
            colorButtonObj.name = "ColorButton";
            BetterButton colorButton = colorButtonObj.GetComponent<BetterButton>();
            AnimateOnPointerEvent pointerEvent = colorButtonObj.GetComponent<AnimateOnPointerEvent>();
            RectTransform rt3 = colorButtonObj.GetComponent<RectTransform>();
            Destroy(colorButtonObj.GetComponent<TextMeshProUGUI>());
            // Scale down & make twice as large to get sharper corners on the buttons
            rt3.anchorMin = new Vector2(1.0f, 0.5f);
            rt3.anchorMax = new Vector2(1.0f, 0.5f);
            rt3.anchoredPosition = new Vector2(-(entryButtonSideMargin + colorButtonSize.x / 2f + resetButtonSize.x + 10f), 0f);
            rt3.sizeDelta = colorButtonSize;
            pointerEvent.defaultScale = Vector3.one;
            pointerEvent.hoverScale = Vector3.one * 1.2f;
            rt3.SetParent(prototype.transform, false);

            GameObject resetButtonObj = Instantiate(CustomizerMod.uiButtonPrototype);
            resetButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false); // disable text
            resetButtonObj.SetActive(true);
            resetButtonObj.GetComponent<Image>().sprite = betterButtonSprite;
            resetButtonObj.name = "ResetButton";
            BetterButton resetButton = resetButtonObj.GetComponent<BetterButton>();
            AnimateOnPointerEvent pointerEvent2 = resetButtonObj.GetComponent<AnimateOnPointerEvent>();
            RectTransform rt4 = resetButtonObj.GetComponent<RectTransform>();
            Image img3 = resetButtonObj.GetComponent<Image>();
            img3.color = resetButtonColor;
            rt4.anchorMin = new Vector2(1.0f, 0.5f);
            rt4.anchorMax = new Vector2(1.0f, 0.5f);
            rt4.sizeDelta = resetButtonSize;
            pointerEvent2.defaultScale = Vector3.one;
            pointerEvent2.hoverScale = Vector3.one * 1.2f;
            rt4.SetParent(prototype.transform, false);
            GameObject resetButtonImgObj = new GameObject("ResetImage");
            RectTransform rt5 = resetButtonImgObj.AddComponent<RectTransform>();
            Image img4 = resetButtonImgObj.AddComponent<Image>();
            img4.sprite = clearSprite;
            img4.color = textColor;
            rt5.anchorMin = Vector2.zero;
            rt5.anchorMax = Vector2.one;
            rt4.anchoredPosition = new Vector2(-(entryButtonSideMargin + resetButtonSize.x / 2f), 0f);
            rt5.sizeDelta = new Vector2(-8f, -8f);
            rt5.SetParent(resetButtonObj.transform, false);

            menuEntry.colorButton = colorButton;
            menuEntry.resetButton = resetButton;
            menuEntry.settingText = tmp;

            prototype.SetActive(false);

            CustomizerPlugin.Logger.LogInfo("Color menu entry prototype created");

            return prototype;
        }

        private void RemoveButtonPersistentListeners(Button button)
        {
            // When a button callback is set in the editor, it is saved as a "persistent" callback and can't be removed from code.
            // Instead of re-making the button from scratch, we can set the callback to not fire.
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }
        }

        private void TurnOnPlayerFocusCamera()
        {
            if (menuCamera == null)
                return;

            Vector3 camPos = EngineHub.PlayerTransforms.controllerRootTransform.TransformPoint(menuCameraPlayerOffset);
            Vector3 camRot = new Vector3(menuCameraPlayerRotation.x, EngineHub.PlayerTransforms.controllerRootTransform.eulerAngles.y + 180f, 0f);
            menuCamera.transform.position = camPos;
            menuCamera.transform.eulerAngles = camRot;
            menuCamera.Priority = 20;
            menuCamera.enabled = true;

            Vector3 diff = menuCamera.transform.position - EngineHub.PlayerTransforms.controllerRootTransform.position;
            playerTargetAngle = Mathf.Atan2(diff.x, diff.z) * 180f / Mathf.PI;
        }

        private void TurnOffMenuCamera()
        {
            if (menuCamera == null)
                return;

            menuCamera.Priority = -10;
            menuCamera.enabled = false;
        }

        private void OnColorButtonPressed(ColorMenuEntry entry)
        {
            if (pickerIsOpen)
            {
                if (selectedEntry != null && selectedEntry == entry)
                {
                    CloseColorPicker();
                }
                else
                {
                    OpenColorPicker(entry);
                }
            }
            else
            {
                OpenColorPicker(entry);
            }
        }

        private void OnResetButtonPressed(ColorMenuEntry entry)
        {
            entry.ResetColor();
            if (pickerIsOpen)
            {
                if (selectedEntry != null && selectedEntry != entry)
                {
                    CloseColorPicker();
                }
                else if (selectedEntry != null)
                {
                    pickerControl.CurrentColor = entry.GetColor();
                }
            }
        }

        private void OpenColorPicker(ColorMenuEntry entry)
        {
            alphaSlider.SetActive(entry.GetDefaultColor().a < 1.0);

            RectTransform entryRect = entry.GetComponent<RectTransform>();
            Vector3 canvasPos = GetComponent<RectTransform>().worldToLocalMatrix.MultiplyPoint3x4(entryRect.position);
            pickerTargetPos = canvasPos.y;

            if (colorEventListener != null)
            {
                pickerControl.onValueChanged.RemoveListener(colorEventListener);
            }
            pickerControl.CurrentColor = entry.GetColor();
            colorEventListener = new UnityAction<Color>(entry.SetColor);
            pickerControl.onValueChanged.AddListener(colorEventListener);

            colorPickerRoot.localScale = menuClosedScale;
            Sequence sequence = pickerOpenSequence;
            if (sequence != null)
            {
                sequence.Kill(false);
            }
            pickerOpenSequence = DOTween.Sequence();
            pickerOpenSequence.Join(colorPickerRoot.DOScale(Vector3.one, menuOpenTime).SetEase(menuOpenEase));
            pickerOpenSequence.Join(pickerCanvasGroup.DOFade(1f, menuOpenTime).SetEase(menuOpenEase));
            pickerOpenSequence.AppendCallback(new TweenCallback(() => pickerCanvasGroup.interactable = true));
            AudioSystem.PlaySound(audioConfig.OpenSound, default(Vector3));

            pickerIsOpen = true;
            selectedEntry = entry;

            StartCoroutine(RecalculatePickerPosition());
        }

        private void CloseColorPicker()
        {
            if (!pickerIsOpen)
                return;

            pickerControl.onValueChanged.RemoveListener(colorEventListener);

            Sequence sequence = pickerOpenSequence;
            if (sequence != null)
            {
                sequence.Kill(false);
            }
            pickerOpenSequence = DOTween.Sequence();
            pickerOpenSequence.AppendCallback(new TweenCallback(() => pickerCanvasGroup.interactable = false));
            pickerOpenSequence.Join(colorPickerRoot.DOScale(menuClosedScale, menuOpenTime).SetEase(menuOpenEase));
            pickerOpenSequence.Join(pickerCanvasGroup.DOFade(0f, menuOpenTime).SetEase(menuOpenEase));

            pickerIsOpen = false;
            selectedEntry = null;
        }

        public IEnumerator RecalculatePickerPosition()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(colorPicker);
            // Center the picker on the button, limiting to screen top/bottom
            float pickerHeight = colorPicker.rect.height;
            RectTransform arrowRect = colorPickerObj.transform.GetChild(1).GetComponent<RectTransform>();
            RectTransform rootRect = colorPickerObj.GetComponent<RectTransform>();
            float clampedPos = Mathf.Clamp(pickerTargetPos, pickerHeight / 2f + marginBottom, canvasSize.y - pickerHeight / 2f - marginTop);
            float posOffset = pickerTargetPos - clampedPos;

            rootRect.localPosition = new Vector3(rootRect.localPosition.x, clampedPos, 0f);
            arrowRect.localPosition = new Vector3(arrowRect.localPosition.x, posOffset, 0f);
        }

        private IEnumerator DelayedApplyAllColors()
        {
            // Wait 1 frame to apply all colors, just in case things aren't quite initialized
            yield return new WaitForEndOfFrame();
            CustomizerMod.ApplyAllPlayerColors();
        }

        private class ColorPageInfo
        {
            public string pageTitle;
            public Dictionary<string, string> settings;
            public Func<bool> canOpenPage;
        }
    }

    
}
