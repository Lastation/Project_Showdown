
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using static BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.ECCurve;

namespace Yamadev.VRCHandMenu
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MenuHandle : UdonSharpBehaviour
    {
        [Header("Main")]
        [SerializeField]
        float menuSize = 0.85f;
        [SerializeField]
        float distance = 0.05f;
        [SerializeField]
        Camera targetCamera;

        [Header("UI")]
        [SerializeField]
        Canvas detailCanvas;
        [SerializeField]
        Canvas tipCanvas;
        [SerializeField]
        Page defaultPage;

        [SerializeField]
        Button returnButton;
        [SerializeField]
        Button closeButton;
        [SerializeField]
        Button pinButton;

        bool _isInitilized = false;
        bool _isVR = false;

        bool _isActive = true;
        float _keyPressTime = 0.0f;
        float _cooling = 0.0f;

        bool _isPined = false;
        bool _showTip = false;
        KeyCode _detectKey = KeyCode.JoystickButton16;

        Page _currentPage;

        [SerializeField]
        MainSystem mainSystem;

        void Start()
        {
            Text tipText = tipCanvas.transform.Find("Text").GetComponent<Text>();

            closeButton.gameObject.SetActive(false);
            pinButton.gameObject.SetActive(false);

            if (Networking.LocalPlayer.IsUserInVR())
            {
                _isVR = true;
                closeButton.gameObject.SetActive(true);
                tipText.text = mainSystem.s_MenuHandle_tipText_VR;
            } else
            {
                pinButton.gameObject.SetActive(true);
                tipText.text = mainSystem.s_MenuHandle_tipText_PC;
            }

            setMenuActive(false);
            setMenuPosition();

            ShowTip();

            _isInitilized = true;
        }

        void Update()
        {
            if (Input.anyKeyDown) Debug.Log(Input.inputString);
            if (Input.GetKeyDown(KeyCode.JoystickButton14) || Input.GetKeyDown(KeyCode.JoystickButton15)) _detectKey = KeyCode.JoystickButton14;
            if (!_isInitilized) return;
            if (!_showTip)
            {
                if (_isVR) detectVRInput();
                else detectDesktopInput();
            }
            setMenuPosition();
        }

        public void ShowTip()
        {
            _showTip = true;

            setMenuActive(true);
            Animator tipAnimator = tipCanvas.transform.Find("Circle/Circle_Fill").GetComponent<Animator>();
            tipCanvas.gameObject.SetActive(true);
            tipAnimator.SetBool("ShowTip", true);

            SendCustomEventDelayedSeconds(nameof(CloseTip), 5.0f);
        }

        public void CloseTip()
        {
            if (!_showTip) return;
            tipCanvas.gameObject.SetActive(false);
            setMenuActive(false);

            _showTip = false;

            if (!_isVR)
            {
                detailCanvas.gameObject.SetActive(true);
                ShowTargetPage(defaultPage);
            }
        }

        void setMenuPosition()
        {
            if (!_isActive) return;
            if (!_isVR)
            {
                float adjustPositionX;
                if (targetCamera != null)
                {
                    Vector3 origin = targetCamera.ViewportToScreenPoint(new Vector3(1, 1, 0));
                    adjustPositionX = origin.x / origin.y / 10;
                }
                else adjustPositionX = 0.18f;

                VRCPlayerApi.TrackingData head = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                Vector3 adjustPosition = new Vector3(adjustPositionX, -0.13f, 0.3f);
                transform.SetPositionAndRotation(head.position + (head.rotation * adjustPosition), head.rotation);
                return;
            }
            VRCPlayerApi.TrackingData hand = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
            var rot = Quaternion.Euler(41.0f, 66.0f, 74.0f);
            var pos = new Vector3(0, 0, distance);
            transform.SetPositionAndRotation(hand.position + (hand.rotation * pos), hand.rotation * rot);
        }

        bool vrKeyDown()
        {
            return Input.GetKeyDown(_detectKey);
        }

        void detectVRInput()
        {
            if (!vrKeyDown()) return;

            var now = Time.time;

            // is cooling
            if (now - _cooling <= 0.8f) return;

            if (now - _keyPressTime > 0.4f || now - _keyPressTime < 0.1f)
            {
                _keyPressTime = now;
                return;
            }

            toggleMenu();
            _cooling = now;
        }

        bool desktopKeyDown()
        {
            return Input.GetKeyDown(KeyCode.Tab);
        }

        bool desktopKeyUp()
        {
            return Input.GetKeyUp(KeyCode.Tab);
        }

        void detectDesktopInput()
        {
            if (_isPined)
            {
                setMenuActive(true);
                return;
            }

            if (desktopKeyDown()) setMenuActive(true);
            if (desktopKeyUp()) setMenuActive(false);
        }

        void setMenuActive(bool active)
        {
            _isActive = active;

            if (active)
            {
                transform.localScale = new Vector3(menuSize, menuSize, 0);
                return;
            }
            transform.localScale = new Vector3(0,0,0);
        }

        void toggleMenu()
        {
            setMenuActive(!_isActive);
        }

        public void HideDetailCanvas()
        {
            if (detailCanvas) detailCanvas.gameObject.SetActive(false);
        }

        public void TogglePin()
        {
            if (_isPined)
            {
                pinButton.transform.Rotate(0, 0, -45.0f);
                _isPined = false;
            } else
            {
                pinButton.transform.Rotate(0, 0, 45.0f);
                _isPined = true;
            }
        }

        public void ShowTargetPage(Page targetPage)
        {
            if (tipCanvas.gameObject.activeSelf) {
                tipCanvas.gameObject.SetActive(false);
                _showTip = false;
            }

            if (detailCanvas) detailCanvas.gameObject.SetActive(true);
            if (_currentPage) _currentPage.gameObject.SetActive(false);

            _currentPage = targetPage;
            targetPage.gameObject.SetActive(true);

            if (!returnButton) return;
            if (targetPage.ParentPage)
            {
                returnButton.gameObject.SetActive(true);
            } else
            {
                returnButton.gameObject.SetActive(false);
            }
        }

        public void ShowParentPage()
        {
            if (!_currentPage.ParentPage) return;
            ShowTargetPage(_currentPage.ParentPage);
        }
    }
}