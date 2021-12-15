
**For a markdown cheat sheet see [Markdown Cheat Sheet](https://www.markdownguide.org/cheat-sheet/)**

## 3D Game Engine Development - [GDLibrary & GDApp](https://github.com/nmcguinness/GD3_3_Intro_To_MonoGame.git)

### Further Reading
- None yet specified

### Explain
- [x] Code additions in Week 6(Halloween)
- [x] UIProgressBarController using Events - Main::Update
- [x] ContentDictionary - Main::modelDictionary and Main::fontDictionary
- [x] UIMenuManager

### Bugs
- [x] Fix Mouse delta on FPC
- [x] GameObject Clone texture sharing - fixed by simplifying creation of Renderer, Mesh, and Shader
- [x] Alpha fade on ui text
- [x] Fix button text not rendering when 2+ buttons
- [x] Fix camera drift on no mouse movement
- [ ] Bug on collider not following Transform rotation
- [ ] Check for bugs on video controller

### Tasks - Week 7
- [x] Add Clone to components
- [x] Add Controller and Behaviour
- [x] Test ModelRenderer
- [x] Add FirstPersonController
- [x] Add Curve
- [x] Add CurveController
- [x] Add XML SerializationUtility
- [x] Add Editor/CurveRecorderController

### Tasks - Week 8
- [x] Re-factor RenderManager to support forward/backward rendering (lighting)
- [x] Re-factor RenderManager and SceneManager as GameComponents - see Main::Update and Draw 
- [x] Add DEMO compiler directive in Main to turn on/off demo code
- [x] Add Round to Vector3 class as an extension
- [x] Round curve recorder translation and rotation stored to XML
- [x] Added support for static and dynamic game objects 
- [x] Re-factor RenderManager and Camera (if necessary) to support multi-screen

### Tasks - Week 9
- [x] Add support for pausing game components to support menu
- [x] Add ui game objects and ui scene manager for onscreen elements
- [x] Add Menu/UI support
- [x] Add EventDispatcher
- [x] Add and improved ContentDictionary with demo Main::modelDictionary and Main::fontDictionary
- [x] Add support to UISceneManager for add/remove ui objects
- [x] Enable lighting turn off for skybox
- [x] Change GameObject::IsStatic to IsPersistent
- [x] Check Transform2D::Clone() for UIObject clones
- [x] Add lock camera to ground
- [x] Add physics engine - Stage 1/3
- [x] Add PhysicsDebugdrawer

### Tasks - Week 10
- [x] Inherit from UIMenuManager to implement app specific menu responses in MyMenuManager
- [x] Add correctly calculated Bounds on UITextureObject and UIButtonObject
- [x] Test layerDepth on UI objects
- [x] Test SoundManager
- [x] Add support for PhysicsManager pause
- [x] Add support for rendering scene in menu background
- [x] Add support for transparent menu items
- [x] Add picking with mouse
- [x] Add support for handling collisions in Collider
- [x] Add events to SoundManager for pause, volume etc
- [x] Add support for adding objects to the scene manager through events
- [x] Add support for removing/adding objects to the ui scene manager through events
- [x] Add support for removing/adding objects to the ui scene manager through events
- [x] Add collidable camera controller


- [ ] Add support for transparent objects
- [ ] Add trianglemesh support - IN BUT BUGGY
- [ ] Add AnimationController
- [ ] Add video controller for GameObjects
- [ ] Add timer scheduler
- [ ] Add RailCamera demo
- [ ] Add Tween functions see [Easing Functions](https://easings.net/)
- [ ] Add lighting engine
- [ ] Add parenting to GameObject and UIObject





