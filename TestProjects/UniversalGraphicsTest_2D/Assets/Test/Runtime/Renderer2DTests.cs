using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[TestFixture]
class Renderer2DTests
{
    GameObject m_BaseObj;
    GameObject m_OverlayObj;
    Camera m_BaseCamera;
    Camera m_OverlayCamera;
    UniversalAdditionalCameraData m_BaseCameraData;
    UniversalAdditionalCameraData m_OverlayCameraData;

    [SetUp]
    public void Setup()
    {
        m_BaseObj = new GameObject();
        m_BaseCamera = m_BaseObj.AddComponent<Camera>();
        m_BaseCameraData = m_BaseObj.AddComponent<UniversalAdditionalCameraData>();

        m_BaseCamera.allowHDR = false;
        m_BaseCameraData.SetRenderer(3);    // 2D Renderer. See the list of Renderers in CommonAssets/UniversalRPAsset.
        m_BaseCameraData.renderType = CameraRenderType.Base;
        m_BaseCameraData.renderPostProcessing = false;

        m_OverlayObj = new GameObject();
        m_OverlayCamera = m_OverlayObj.AddComponent<Camera>();
        m_OverlayCameraData = m_OverlayObj.AddComponent<UniversalAdditionalCameraData>();

        m_OverlayCamera.allowHDR = false;
        m_OverlayCameraData.SetRenderer(3);    // 2D Renderer. See the list of Renderers in CommonAssets/UniversalRPAsset.
        m_OverlayCameraData.renderType = CameraRenderType.Overlay;
        m_OverlayCameraData.renderPostProcessing = false;
    }

    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(m_OverlayObj);
        Object.DestroyImmediate(m_BaseObj);
    }

    [Test]
    public void BaseRendererDoesNotCreateRenderTexturesIfStackIsEmpty()
    {
        m_BaseCamera.Render();

        Renderer2D baseRenderer = m_BaseCameraData.scriptableRenderer as Renderer2D;

        // XRTODO: investigate why baseRenderer.createColorTexture (due to sRGB) is true when XR is enabled
        if (UnityEngine.Rendering.XRGraphicsAutomatedTests.enabled)
            return;

        Assert.IsFalse(baseRenderer.createColorTexture);

        Assert.IsFalse(baseRenderer.createDepthTexture);
    }

    [Test]
    public void BaseRendererCreatesRenderTexturesIfStackIsNotEmpty()
    {
        m_BaseCameraData.cameraStack.Add(m_OverlayCamera);

        m_BaseCamera.Render();

        Renderer2D baseRenderer = m_BaseCameraData.scriptableRenderer as Renderer2D;

        Assert.IsTrue(baseRenderer.createColorTexture);

        Assert.IsTrue(baseRenderer.createDepthTexture);
    }

    [Test]
    public void BaseRendererUsesDepthAttachmentOfColorTextureIfNoDepthTextureCreated()
    {
        m_BaseCameraData.renderPostProcessing = true;   // This will make the renderer create color texture.

        m_BaseCamera.Render();

        Renderer2D baseRenderer = m_BaseCameraData.scriptableRenderer as Renderer2D;

        Assert.IsTrue(baseRenderer.createColorTexture);

        Assert.IsFalse(baseRenderer.createDepthTexture);
    }

    [Test]
    public void OverlayRendererUsesRenderTexturesFromBase()
    {
        m_BaseCameraData.cameraStack.Add(m_OverlayCamera);

        m_BaseCamera.Render();

        Renderer2D baseRenderer = m_BaseCameraData.scriptableRenderer as Renderer2D;
        Renderer2D overlayRenderer = m_OverlayCameraData.scriptableRenderer as Renderer2D;
    }

    [Test]
    public void OverlayRendererSetsTheCreateTextureFlags()
    {
        m_BaseCameraData.cameraStack.Add(m_OverlayCamera);

        m_BaseCamera.Render();

        Renderer2D overlayRenderer = m_OverlayCameraData.scriptableRenderer as Renderer2D;

        Assert.IsTrue(overlayRenderer.createColorTexture);
        Assert.IsTrue(overlayRenderer.createDepthTexture);
    }
}
