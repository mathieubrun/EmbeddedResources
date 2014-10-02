<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EmbeddedResourceHandler.Sample.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>FromExternal/Data/Hello.txt</p>
    <iframe src="FromExternal/Data/Hello.txt" height="100" width="400" ></iframe>
        <hr />
        <p>FromSame/Data/Hello.txt</p>
    <iframe src="FromSame/Data/Hello.txt" height="100" width="400" ></iframe>
        <hr />
        <p>Data/Hello.txt</p>
    <iframe src="Data/Hello.txt" height="100" width="400" ></iframe>

    </div>
    </form>
</body>
</html>
