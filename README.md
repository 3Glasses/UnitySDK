<div id="table-of-contents">
<h2>Table of Contents</h2>
<div id="text-table-of-contents">
<ul>
<li><a href="#sec-1">1. 3GlassesSDK</a>
<ul>
<li><a href="#sec-1-1">1.1. How to Use</a>
<ul>
<li><a href="#sec-1-1-1">1.1.1. Properties:</a></li>
</ul>
</li>
<li><a href="#sec-1-2">1.2. Get HMD Info</a></li>
<li><a href="#sec-1-3">1.3. Get Wand Input</a>
<ul>
<li><a href="#sec-1-3-1">1.3.1. Get</a></li>
<li><a href="#sec-1-3-2">1.3.2. Bind Script</a></li>
</ul>
</li>
</ul>
</li>
</ul>
</div>
</div>

![img](./README/icon.png)

# 3GlassesSDK<a id="sec-1" name="sec-1"></a>

**[Home Page](http://dev.vrshow.com/)** | **[中文手册](file://d:/README_zh.md)**

## How to Use<a id="sec-1-1" name="sec-1-1"></a>

Select 3Glasses>EnableHeadDisplay from the main menu.

![img](./README/EnableHeadDisplay.png)

Make sure the EnableHeadDisplay is selected in the 3Glasses drop-down box.

Now the Three3GlassesManager script is bind in your main camera, and there are some Properties can control the HeadDisplay's behavior.

![img](./README/TreeGlassesCameraProperty.png)

### Properties:<a id="sec-1-1-1" name="sec-1-1-1"></a>

<table border="2" cellspacing="0" cellpadding="6" rules="all" frame="border">


<colgroup>
<col  class="left" />

<col  class="left" />
</colgroup>
<thead>
<tr>
<th scope="col" class="left">Property:</th>
<th scope="col" class="left">Discription</th>
</tr>
</thead>

<tbody>
<tr>
<td class="left">Clone Target Camera</td>
<td class="left">The VR camera will clone the parameters of the specified camera (mandatory, not empty)</td>
</tr>


<tr>
<td class="left">Bind Target Camera</td>
<td class="left">Bind the rotation and displacement of the camera to the main camera ( don't check to bind the rotation and displacement to the current GameObject)</td>
</tr>


<tr>
<td class="left">FreezePosition</td>
<td class="left">freeze the headdisplay's position</td>
</tr>


<tr>
<td class="left">FreezeRotation</td>
<td class="left">freeze the headdisplay's rotation</td>
</tr>


<tr>
<td class="left">Hmd Anti Aliasing Level</td>
<td class="left">RenderTexture's Anti Aliasing Level</td>
</tr>


<tr>
<td class="left">Eye Distance</td>
<td class="left">The distance between the left and right camera in the HeadDisplay.</td>
</tr>


<tr>
<td class="left">Layer Mask</td>
<td class="left">Includes or omits layers of objects to be rendered by the HeadDisplay device</td>
</tr>


<tr>
<td class="left">Enable JoyPad</td>
<td class="left">Enable the 3Glasses Wand</td>
</tr>
</tbody>
</table>

## Get HMD Info<a id="sec-1-2" name="sec-1-2"></a>

-   Get HMD's position and rotation
    
        using ThreeGlasses;
        
        //...
        
        Vector3 pos = TGInput.GetPosition(InputType.HMD);
        Quaternion rotate = TGInput.GetRotation(InputType.HMD);

-   Get HMD's button and touchpad info
    
        // if the HMD's Menu Button is pressed
        TGInput.GetKey(InputType.HMD, InputKey.HmdMenu);
        
        // get touchpad info，rang is [-1.0~1.0]
        Vector2 v = TGInput.GetHMDTouchPad();

## Get Wand Input<a id="sec-1-3" name="sec-1-3"></a>

There are two ways to get wand's info:

### Get<a id="sec-1-3-1" name="sec-1-3-1"></a>

The InputExtendMethods class definit some methods for get the wand's info. you can get the info like this:

    using ThreeGlasses;
    
    //...
    
    // get the back key status (down is true)
    TGInput.GetKey(InputType.LeftWand, InputKey.WandBack);
    // get the stick's info,Both the X axis and the Y axis are limited to between -1 and 1.
    Vector2 v = TGInput.GetStick(InputType.LeftWand);

### Bind Script<a id="sec-1-3-2" name="sec-1-3-2"></a>

Bind the ThreeGlassesWandBind.cs on your wand object.

![img](./README/TreeGlassesWandBindProperty.png)

Option

<table border="2" cellspacing="0" cellpadding="6" rules="all" frame="border">


<colgroup>
<col  class="left" />

<col  class="left" />
</colgroup>
<thead>
<tr>
<th scope="col" class="left">Option</th>
<th scope="col" class="left">Description</th>
</tr>
</thead>

<tbody>
<tr>
<td class="left">Type</td>
<td class="left">wand's type</td>
</tr>


<tr>
<td class="left">Send To Children</td>
<td class="left">send wand's info to all children</td>
</tr>


<tr>
<td class="left">Update Self</td>
<td class="left">update the position and rotation</td>
</tr>


<tr>
<td class="left">Move Scale</td>
<td class="left">scale wand's position</td>
</tr>


<tr>
<td class="left">Update Type</td>
<td class="left">update local or world</td>
</tr>
</tbody>
</table>

The children of the object who bind the ThreeGlassesWandBind.cs can implement OnWandChange for get the wand's info.

    void OnWandChange(ThreeGlassesWand.Wand pack)
    {
        // do somethng
        // pack.position or rotation
        // pack.GetKey(InputKey.xxx);
    }