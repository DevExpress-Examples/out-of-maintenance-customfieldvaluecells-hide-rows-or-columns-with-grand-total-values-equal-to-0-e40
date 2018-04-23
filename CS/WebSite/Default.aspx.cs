using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraPivotGrid;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        if(!IsCallback && !IsPostBack) {
            PivotHelper.FillPivot(pivotGrid);
            FillRadioItems();
        }
        pivotGrid.DataSource = PivotHelper.GetDataTable();
    }
    void FillRadioItems() {
        for(int i = 0; i < CustomFieldValueHelper.Captions.Length; i++) {
            radioButtonList.Items.Add(new ListItem(CustomFieldValueHelper.Captions[i], Convert.ToString(i)));
        }
        radioButtonList.SelectedIndex = 0;
    }
    protected void pivotGrid_CustomFieldValueCells(object sender, PivotCustomFieldValueCellsEventArgs e) {
        if(pivotGrid.DataSource == null) return;
        if (radioButtonList.SelectedIndex == 1)
            CustomFieldValueHelper.ApplyCustomFieldValueCells( e, pivotGrid.Fields[PivotHelper.Remains]);
    }
    protected void pivotGrid_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
        if( Object.Equals( e.Field, pivotGrid.Fields[PivotHelper.Month])) {
            e.DisplayText = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)e.Value);
        }
    }
}

public static class CustomFieldValueHelper {
    public const int Count = 2;
    public static string[] Captions = new string[Count] { "Show Everything", "Delete rows with 0 Grand Total" };

    public static void ApplyCustomFieldValueCells( PivotCustomFieldValueCellsEventArgs e, PivotGridField dataField ) {
        bool isColumn = false;
        int levelCount = e.GetLevelCount(isColumn);
        for(int i = e.GetCellCount(isColumn) - 1; i >= 0; i--) {
            FieldValueCell crossCell = e.GetCell(isColumn, i);
            if(crossCell.EndLevel != levelCount - 1) continue;
            for(int j = e.GetCellCount(!isColumn) - 1; j >= 0; j--) {
                FieldValueCell cell = e.GetCell(!isColumn, j);
                if(IsCellValid(cell, dataField)) {
                    object dataCellValue = isColumn ? e.GetCellValue(crossCell.MinIndex, cell.MinIndex) : e.GetCellValue(cell.MinIndex, crossCell.MinIndex);
                    if(object.Equals(dataCellValue, 0m))
                        e.Remove(crossCell);
                }
            }
        }
    }
    static bool IsCellValid(FieldValueCell cell, PivotGridField field) {
        return cell.ValueType == PivotGridValueType.GrandTotal && Object.Equals( cell.DataField, field);
    }
}

public static class PivotHelper {
    public const string Employee = "Employee";
    public const string Widget = "Widget";
    public const string Month = "Month";
    public const string RetailPrice = "Retail Price";
    public const string WholesalePrice = "Wholesale Price";
    public const string Quantity = "Quantity";
    public const string Remains = "Remains";

    public const string EmployeeA = Employee + " A";
    public const string EmployeeB = Employee + " B";
    public const string WidgetA = Widget + " A";
    public const string WidgetB = Widget + " B";
    public const string WidgetC = Widget + " C";

    public static void FillPivot(ASPxPivotGrid pivot) {
        pivot.Fields.Add(Employee, PivotArea.RowArea);
        pivot.Fields.Add(Widget, PivotArea.RowArea);
        pivot.Fields.Add(Month, PivotArea.ColumnArea).AreaIndex = 0;
        pivot.Fields.Add(RetailPrice, PivotArea.DataArea);
        pivot.Fields.Add(WholesalePrice, PivotArea.DataArea);
        pivot.Fields.Add(Quantity, PivotArea.DataArea);
        pivot.Fields.Add(Remains, PivotArea.DataArea);
        foreach(PivotGridField field in pivot.Fields) {
            field.AllowedAreas = GetAllowedArea(field.Area);
        }
        pivot.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Far;
        pivot.OptionsView.ColumnTotalsLocation = PivotTotalsLocation.Far;
        pivot.OptionsDataField.Area = PivotDataArea.ColumnArea;
        pivot.OptionsDataField.AreaIndex = 1;
    }
    static PivotGridAllowedAreas GetAllowedArea(PivotArea area) {
        switch(area) {
            case PivotArea.ColumnArea:
                return PivotGridAllowedAreas.ColumnArea;
            case PivotArea.RowArea:
                return PivotGridAllowedAreas.RowArea;
            case PivotArea.DataArea:
                return PivotGridAllowedAreas.DataArea;
            case PivotArea.FilterArea:
                return PivotGridAllowedAreas.FilterArea;
            default:
                return PivotGridAllowedAreas.All;
        }
    }
    public static DataTable GetDataTable() {
        DataTable table = new DataTable();
        table.Columns.Add(Employee, typeof(string));
        table.Columns.Add(Widget, typeof(string));
        table.Columns.Add(Month, typeof(int));
        table.Columns.Add(RetailPrice, typeof(double));
        table.Columns.Add(WholesalePrice, typeof(double));
        table.Columns.Add(Quantity, typeof(int));
        table.Columns.Add(Remains, typeof(int));
        table.Rows.Add(EmployeeA, WidgetA, 6, 45.6, 40, 3, 7);
        table.Rows.Add(EmployeeA, WidgetA, 7, 38.9, 30, 6, 1);
        table.Rows.Add(EmployeeA, WidgetB, 6, 24.7, 20, 7, -3);
        table.Rows.Add(EmployeeA, WidgetB, 7, 8.3, 7.5, 5, 3);
        table.Rows.Add(EmployeeA, WidgetC, 6, 10.0, 9, 4, 0);
        table.Rows.Add(EmployeeA, WidgetC, 7, 20.0, 18.5, 5, 1);
        table.Rows.Add(EmployeeB, WidgetA, 6, 77.8, 70, 2, 0);
        table.Rows.Add(EmployeeB, WidgetA, 7, 32.5, 30, 1, 1);
        table.Rows.Add(EmployeeB, WidgetB, 6, 12, 11, 10, 0);
        table.Rows.Add(EmployeeB, WidgetB, 7, 6.7, 5.5, 4, 1);
        table.Rows.Add(EmployeeB, WidgetC, 6, 30.0, 28.7, 6, 0);
        table.Rows.Add(EmployeeB, WidgetC, 7, 40.0, 38.3, 7, 0);
        return table;
    }
}