<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="ImagePaste.cs" Inherits="Dialogs_ProfileImage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<script src="../Js/imagepaste.js" type="text/javascript"></script>
<script src="/frontend/library/jquery.Jcrop.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
        <div class="image-cropper">
            <div id="mainImageDiv" style="width:700px;height:250px;margin:0 auto;overflow:auto">
                <img id="testImage" class="image" style="opacity:0" src="" />
            </div>
            <asp:Button id="resizeButton" runat="server" Text="Resize Image" OnClick="resizeButton_Click1" />
            <div id="imageOptions" style="display:none">
                <div style="text-align:center">
                    <button id="cropButton">Crop Image</button>
                    
                    <button id="clearButton">Clear Image</button>
                </div>

                <br />
                
                <div style="text-align:center;display:none">
                    <input type="text" id="imgWidth" placeholder="width" />
                    <span>x</span>
                    <input type="text" id="imgHeight" placeholder="height" />   
                    <button id="saveResizeButton">Resize</button>                     
                </div>                
                <div>Preview your image:</div>
                <div style="text-align:center; ">
                
                    <div class="preview" >
                    </div>

                    <input id='img1' type="hidden" class="result" value="" runat="server" />
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>

