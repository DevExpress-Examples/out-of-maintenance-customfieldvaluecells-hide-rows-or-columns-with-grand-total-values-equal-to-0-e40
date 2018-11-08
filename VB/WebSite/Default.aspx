<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
		<asp:RadioButtonList ID="radioButtonList" runat="server" AutoPostBack="true" CellSpacing="10" />
		<br />
		<div>
			<dx:ASPxPivotGrid ID="pivotGrid" runat="server" Width="500px" 
				OnFieldValueDisplayText="pivotGrid_FieldValueDisplayText" OnCustomFieldValueCells="pivotGrid_CustomFieldValueCells" />
		</div>
	</form>    
</body>

</html>