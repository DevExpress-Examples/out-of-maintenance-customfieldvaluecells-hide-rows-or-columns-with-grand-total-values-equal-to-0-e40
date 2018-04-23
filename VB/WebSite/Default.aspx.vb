Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxPivotGrid
Imports DevExpress.XtraPivotGrid

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If (Not IsCallback) AndAlso (Not IsPostBack) Then
			PivotHelper.FillPivot(pivotGrid)
			FillRadioItems()
		End If
		pivotGrid.DataSource = PivotHelper.GetDataTable()
	End Sub
	Private Sub FillRadioItems()
		For i As Integer = 0 To CustomFieldValueHelper.Captions.Length - 1
			radioButtonList.Items.Add(New ListItem(CustomFieldValueHelper.Captions(i), Convert.ToString(i)))
		Next i
		radioButtonList.SelectedIndex = 0
	End Sub
	Protected Sub pivotGrid_CustomFieldValueCells(ByVal sender As Object, ByVal e As PivotCustomFieldValueCellsEventArgs)
		If pivotGrid.DataSource Is Nothing Then
			Return
		End If
		If radioButtonList.SelectedIndex = 1 Then
			CustomFieldValueHelper.ApplyCustomFieldValueCells(e, pivotGrid.Fields(PivotHelper.Remains))
		End If
	End Sub
	Protected Sub pivotGrid_FieldValueDisplayText(ByVal sender As Object, ByVal e As PivotFieldDisplayTextEventArgs)
		If Object.Equals(e.Field, pivotGrid.Fields(PivotHelper.Month)) Then
			e.DisplayText = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(CInt(Fix(e.Value)))
		End If
	End Sub
End Class

Public NotInheritable Class CustomFieldValueHelper
	Public Const Count As Integer = 2
	Public Shared Captions() As String = { "Show Everything", "Delete rows with 0 Grand Total" }

	Private Sub New()
	End Sub
	Public Shared Sub ApplyCustomFieldValueCells(ByVal e As PivotCustomFieldValueCellsEventArgs, ByVal dataField As PivotGridField)
		Dim isColumn As Boolean = False
		Dim levelCount As Integer = e.GetLevelCount(isColumn)
		For i As Integer = e.GetCellCount(isColumn) - 1 To 0 Step -1
			Dim crossCell As FieldValueCell = e.GetCell(isColumn, i)
			If crossCell.EndLevel <> levelCount - 1 Then
				Continue For
			End If
			For j As Integer = e.GetCellCount((Not isColumn)) - 1 To 0 Step -1
				Dim cell As FieldValueCell = e.GetCell((Not isColumn), j)
				If IsCellValid(cell, dataField) Then
					Dim dataCellValue As Object
					If isColumn Then
						dataCellValue = e.GetCellValue(crossCell.MinIndex, cell.MinIndex)
					Else
						dataCellValue = e.GetCellValue(cell.MinIndex, crossCell.MinIndex)
					End If
					If Object.Equals(dataCellValue, 0D) Then
						e.Remove(crossCell)
					End If
				End If
			Next j
		Next i
	End Sub
	Private Shared Function IsCellValid(ByVal cell As FieldValueCell, ByVal field As PivotGridField) As Boolean
		Return cell.ValueType = PivotGridValueType.GrandTotal AndAlso Object.Equals(cell.DataField, field)
	End Function
End Class

Public NotInheritable Class PivotHelper
	Public Const Employee As String = "Employee"
	Public Const Widget As String = "Widget"
	Public Const Month As String = "Month"
	Public Const RetailPrice As String = "Retail Price"
	Public Const WholesalePrice As String = "Wholesale Price"
	Public Const Quantity As String = "Quantity"
	Public Const Remains As String = "Remains"

	Public Const EmployeeA As String = Employee & " A"
	Public Const EmployeeB As String = Employee & " B"
	Public Const WidgetA As String = Widget & " A"
	Public Const WidgetB As String = Widget & " B"
	Public Const WidgetC As String = Widget & " C"

	Private Sub New()
	End Sub
	Public Shared Sub FillPivot(ByVal pivot As ASPxPivotGrid)
		pivot.Fields.Add(Employee, PivotArea.RowArea)
		pivot.Fields.Add(Widget, PivotArea.RowArea)
		pivot.Fields.Add(Month, PivotArea.ColumnArea).AreaIndex = 0
		pivot.Fields.Add(RetailPrice, PivotArea.DataArea)
		pivot.Fields.Add(WholesalePrice, PivotArea.DataArea)
		pivot.Fields.Add(Quantity, PivotArea.DataArea)
		pivot.Fields.Add(Remains, PivotArea.DataArea)
		For Each field As PivotGridField In pivot.Fields
			field.AllowedAreas = GetAllowedArea(field.Area)
		Next field
		pivot.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Far
		pivot.OptionsView.ColumnTotalsLocation = PivotTotalsLocation.Far
		pivot.OptionsDataField.Area = PivotDataArea.ColumnArea
		pivot.OptionsDataField.AreaIndex = 1
	End Sub
	Private Shared Function GetAllowedArea(ByVal area As PivotArea) As PivotGridAllowedAreas
		Select Case area
			Case PivotArea.ColumnArea
				Return PivotGridAllowedAreas.ColumnArea
			Case PivotArea.RowArea
				Return PivotGridAllowedAreas.RowArea
			Case PivotArea.DataArea
				Return PivotGridAllowedAreas.DataArea
			Case PivotArea.FilterArea
				Return PivotGridAllowedAreas.FilterArea
			Case Else
				Return PivotGridAllowedAreas.All
		End Select
	End Function
	Public Shared Function GetDataTable() As DataTable
		Dim table As New DataTable()
		table.Columns.Add(Employee, GetType(String))
		table.Columns.Add(Widget, GetType(String))
		table.Columns.Add(Month, GetType(Integer))
		table.Columns.Add(RetailPrice, GetType(Double))
		table.Columns.Add(WholesalePrice, GetType(Double))
		table.Columns.Add(Quantity, GetType(Integer))
		table.Columns.Add(Remains, GetType(Integer))
		table.Rows.Add(EmployeeA, WidgetA, 6, 45.6, 40, 3, 7)
		table.Rows.Add(EmployeeA, WidgetA, 7, 38.9, 30, 6, 1)
		table.Rows.Add(EmployeeA, WidgetB, 6, 24.7, 20, 7, -3)
		table.Rows.Add(EmployeeA, WidgetB, 7, 8.3, 7.5, 5, 3)
		table.Rows.Add(EmployeeA, WidgetC, 6, 10.0, 9, 4, 0)
		table.Rows.Add(EmployeeA, WidgetC, 7, 20.0, 18.5, 5, 1)
		table.Rows.Add(EmployeeB, WidgetA, 6, 77.8, 70, 2, 0)
		table.Rows.Add(EmployeeB, WidgetA, 7, 32.5, 30, 1, 1)
		table.Rows.Add(EmployeeB, WidgetB, 6, 12, 11, 10, 0)
		table.Rows.Add(EmployeeB, WidgetB, 7, 6.7, 5.5, 4, 1)
		table.Rows.Add(EmployeeB, WidgetC, 6, 30.0, 28.7, 6, 0)
		table.Rows.Add(EmployeeB, WidgetC, 7, 40.0, 38.3, 7, 0)
		Return table
	End Function
End Class