using System.Linq;

using UnityEditor.Experimental;
using UnityEditor.PackageManager.UI;

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.VFX.UI
{
    class VFXHelpDropdownButton : DropDownButtonBase
    {
        const string k_PackageName = "com.unity.visualeffectgraph";
        const string k_AdditionalSamples = "VisualEffectGraph Additions";
        const string k_AdditionalHelpers = "OutputEvent Helpers";
        const string k_ManualUrl = @"https://docs.unity3d.com/Packages/com.unity.visualeffectgraph@latest/index.html";
        const string k_ForumUrl = @"https://forum.unity.com/forums/visual-effect-graph.428/";
        const string k_SpaceShipUrl = @"https://github.com/Unity-Technologies/SpaceshipDemo";
        const string k_SamplesUrl = @"https://github.com/Unity-Technologies/VisualEffectGraph-Samples";
        const string k_VfxGraphUrl = @"https://unity.com/visual-effect-graph";

        readonly VFXView m_VFXView;
        readonly Button m_installSamplesButton;
        readonly Button m_installHelpersButton;

        public VFXHelpDropdownButton(VFXView vfxView, VFXViewWindow parentWindow)
            : base(
                parentWindow,
                "VFXHelpDropdownPanel",
                "Open the user manual of Visual Effect Graph",
                "help-button",
                EditorResources.iconsPath + "_Help.png",
                true)
        {
            m_VFXView = vfxView;

            m_installSamplesButton = m_PopupContent.Q<Button>("installSamples");
            m_installSamplesButton.clicked += OnInstallSamples;

            m_installHelpersButton = m_PopupContent.Q<Button>("graphAddition");
            m_installHelpersButton.clicked += OnInstallGraphAddition;

            var gotoHome = m_PopupContent.Q<Button>("gotoHome");
            gotoHome.clicked += () => GotoUrl(k_VfxGraphUrl);

            var gotoForum = m_PopupContent.Q<Button>("gotoForum");
            gotoForum.clicked += () => GotoUrl(k_ForumUrl);

            var gotoSpaceShip = m_PopupContent.Q<Button>("gotoSpaceShip");
            gotoSpaceShip.clicked += () => GotoUrl(k_SpaceShipUrl);

            var gotoSamples = m_PopupContent.Q<Button>("gotoSamples");
            gotoSamples.clicked += () => GotoUrl(k_SamplesUrl);
        }

        protected override Vector2 GetPopupSize() => new Vector2(200, 224);

        protected override void OnMainButton()
        {
            GotoUrl(k_ManualUrl);
        }

        void GotoUrl(string url) => Help.BrowseURL(url);

        void OnInstallSamples()
        {
            InstallSample(k_AdditionalSamples);
        }

        void OnInstallGraphAddition()
        {
            InstallSample(k_AdditionalHelpers);
        }

        void InstallSample(string sampleName)
        {
            var sample = Sample.FindByPackage(k_PackageName, null).SingleOrDefault(x => x.displayName == sampleName);
            if (!string.IsNullOrEmpty(sample.displayName))
            {
                if (!sample.isImported)
                {
                    sample.Import();
                }
                else
                {
                    var reinstall = EditorUtility.DisplayDialog("Warning", "This sample package is already installed.\nDo you want to reinstall it?", "Yes", "No");
                    if (reinstall)
                    {
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Could not find sample package {sampleName}");
            }
        }
    }
}
