<div id="table-of-contents">
<h2>Table of Contents</h2>
<div id="text-table-of-contents">
<ul>
<li><a href="#sec-1">1. 3GlassesSDK</a>
<ul>
<li><a href="#sec-1-1">1.1. 接入SDK</a></li>
<li><a href="#sec-1-2">1.2. 使用SDK脚本</a></li>
</ul>
</li>
</ul>
</div>
</div>

![img](./README/icon.png)

# 3GlassesSDK<a id="sec-1" name="sec-1"></a>

**[开发者主页](http://dev.vrshow.com/)** | **[English Manual](./README.md)**

## 接入SDK<a id="sec-1-1" name="sec-1-1"></a>

-   如果之前有安装过3Glasses的SDK，请先在项目中删除干净。
-   导入release的unity包，或直接将Assets/ThreeGlasses拷贝到自己项目的Assests文件夹下。
-   这时可以在menu菜单中看到3Glasses的menu，如果没有重复以上步骤。
-   从menu菜单中选择 3Glasses>EnableHeadDisplay
    
    ![img](./README/EnableHeadDisplay.png)
    
    确保EnableHeadDisplay菜单选项已被勾选。
-   现在Three3GlassesManager脚本已经绑定到你的主相机上了。
    
    ![img](./README/TreeGlassesCameraProperty.png)
    
    <table border="2" cellspacing="0" cellpadding="6" rules="all" frame="border">
    
    
    <colgroup>
    <col  class="left" />
    
    <col  class="left" />
    </colgroup>
    <thead>
    <tr>
    <th scope="col" class="left">参数</th>
    <th scope="col" class="left">描述</th>
    </tr>
    </thead>
    
    <tbody>
    <tr>
    <td class="left">Clone Target Camera</td>
    <td class="left">VR相机将克隆指定相机的参数(必选,不能为空)</td>
    </tr>
    
    
    <tr>
    <td class="left">Bind Target Camera</td>
    <td class="left">将相机的旋转和位移绑定到主相机上(不勾选则将旋转和位移绑定到当前GameObject上)</td>
    </tr>
    
    
    <tr>
    <td class="left">Freeze Position</td>
    <td class="left">头盔的位置变动不会更新</td>
    </tr>
    
    
    <tr>
    <td class="left">Freeze Rotation</td>
    <td class="left">头盔的旋转变动不会更新</td>
    </tr>
    
    
    <tr>
    <td class="left">Hmd Anti Aliasing Level</td>
    <td class="left">渲染的抗锯齿级别</td>
    </tr>
    
    
    <tr>
    <td class="left">Eye Distance</td>
    <td class="left">头显内部左右相机的距离</td>
    </tr>
    
    
    <tr>
    <td class="left">Layer Mask</td>
    <td class="left">头显可以看到的层</td>
    </tr>
    
    
    <tr>
    <td class="left">Enable JoyPad</td>
    <td class="left">使用3Glasses的Wand手柄</td>
    </tr>
    </tbody>
    </table>

## 使用SDK脚本<a id="sec-1-2" name="sec-1-2"></a>

-   获取头显的位置和旋转信息
    
        using ThreeGlasses;
        
        //...
        
        Vector3 pos = TGInput.GetPosition(InputType.HMD);
        Quaternion rotate = TGInput.GetRotation(InputType.HMD);
    
    或者可以直接获取Camera的Transform。

-   获取头显的按键和TouchPad信息
    
        // 这里获取头显的menu按键是否有按下
        TGInput.GetKey(InputType.HMD, InputKey.HmdMenu);
        
        // 获取touchpad信息，范围是 [-1.0~1.0]
        Vector2 v = TGInput.GetHMDTouchPad();

-   获取Wand手柄的输入
    有两种方法获取手柄信息：
    -   直接Get的方式
        InputExtendMethods类定义了一些获取手柄信息的方法，如下：
        
            using ThreeGlasses;
            
            //...
            
            // 这里是获取wand手柄上back按键的状态 (按键按下返回true)
            TGInput.GetKey(InputType.LeftWand, InputKey.WandBack);
            // 返回遥感的x,y值，这两个值都在-1--1之间
            Vector2 v = TGInput.GetStick(InputType.LeftWand);
    -   绑定脚本方式
        将ThreeGlassesWandBind.cs脚本绑定到GameObject上。
        
        ![img](./README/TreeGlassesWandBindProperty.png)
        
        <table border="2" cellspacing="0" cellpadding="6" rules="all" frame="border">
        
        
        <colgroup>
        <col  class="left" />
        
        <col  class="left" />
        </colgroup>
        <thead>
        <tr>
        <th scope="col" class="left">参数</th>
        <th scope="col" class="left">描述</th>
        </tr>
        </thead>
        
        <tbody>
        <tr>
        <td class="left">Type</td>
        <td class="left">手柄类型</td>
        </tr>
        
        
        <tr>
        <td class="left">Send To Children</td>
        <td class="left">是否将获取的手柄信息传递给子对象</td>
        </tr>
        
        
        <tr>
        <td class="left">Update Self</td>
        <td class="left">是否更新自己的位置和旋转</td>
        </tr>
        
        
        <tr>
        <td class="left">Move Scale</td>
        <td class="left">手柄移动的放大系数</td>
        </tr>
        
        
        <tr>
        <td class="left">Update Type</td>
        <td class="left">更新本地坐标还是世界坐标</td>
        </tr>
        </tbody>
        </table>
        
        手柄对象的子对象只要实现OnWandChange方法就可以获取手柄的详细信息了。
        
            void OnWandChange(ThreeGlassesWand.Wand pack)
            {
                // do somethng
                // pack.position or rotation
                // pack.GetKey(InputKey.xxx);
            }