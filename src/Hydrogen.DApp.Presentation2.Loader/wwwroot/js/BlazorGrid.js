function ConsoleWrite(data) {
    console.log(data);
}

function AlertWrite(data) {
    alert(data);
}

var MinimumColumWidth = 20;

let TableIds = new Map();
let ColumnWidths = new Map();
let CurColIndexes = new Map();
let CurCols = new Map();

function ResizableColumnTable(id, csharpInstance)
{
    var table = document.getElementById(id);
    if (table === null) return;

    TableIds.set(id, table);

    CSharpInstance = csharpInstance;
    ResizableTable(table, id);
}

function ResizableTable(table, id) {

    //console.log("ResizableTable: " + id);

    var row = table.getElementsByTagName('tr')[0],
        cols = row ? row.children : undefined;
    if (!cols) return;

    var columnWidths = new Array(cols.length);
    for (var i = 0; i < cols.length; i++) {
        columnWidths[i] = cols[i].clientWidth;
    }

    // save the column widths for this table
    ColumnWidths.set(id, columnWidths);

    table.style.overflow = 'hidden';
    var headerHeight = $(table).find('thead').height();

    for (var i = 0; i < cols.length; i++) {
        var div = createDiv(headerHeight);
        cols[i].appendChild(div);
        cols[i].style.position = 'relative';
        setListeners(div, id);
    }

    function setListeners(div, id) {
        div.addEventListener('mousedown', function (e) {
            var curCol = e.target.parentElement;
            var curColIndex = curCol.cellIndex;

            CurCols.set(id, curCol);
            CurColIndexes.set(id, curColIndex);
        });

        div.addEventListener('mouseover', function (e) {
            e.target.style.borderRight = '5px solid #2c7ec9';
        })

        div.addEventListener('mouseout', function (e) {
            e.target.style.borderRight = '';
        })

        document.addEventListener('mousemove', function (e) {

            if (!CurColIndexes.has(id)) {
                return;
            }

            var curColIndex = CurColIndexes.get(id);
            var curCol = CurCols.get(id);
            var columnWidths = ColumnWidths.get(id);

            if (curCol) {
                var mouseX = e.pageX;
                var table = TableIds.get(id);
                var tableLeft = table.getBoundingClientRect().left;
                var inBetweenColumnsWidth = 0;

                for (var i = 0; i < curColIndex; i++) {
                    inBetweenColumnsWidth += columnWidths[i];
                }

                var newWidth = (mouseX - tableLeft) - inBetweenColumnsWidth;

                if (newWidth < MinimumColumWidth) newWidth = MinimumColumWidth;

                //console.log("mouseX: " + mouseX + " Table left: " + tableLeft + " inBetweenColumnsWidth: " + inBetweenColumnsWidth + " newWidth: " + newWidth + "curColIndex: " + curColIndex);

                // update the current column's width in the global ColumnWidths
                columnWidths[curColIndex] = newWidth;

                // set every row's cell's width
                for (var row = 0; row < table.rows.length; row++) {
                    for (var column = 0; column < columnWidths.length; column++) {
                        table.rows[row].cells[column].style.width = columnWidths[column] + 'px';
                    }
                }
            }
        });

        document.addEventListener('mouseup', function (e)
        {
            // call a static C# function
            //DotNet.invokeMethodAsync("SystemX", "SaveColumnWidth", curColTitle, newWidth);

            // call an instance C# function
            // CSharpInstance.invokeMethodAsync("SaveColumnWidth", curColTitle, newWidth);

            CurColIndexes.delete(id);
            CurCols.delete(id);
        });
    }

    function createDiv(height)
    {
        var div = document.createElement('div');
        div.style.top = 0;
        div.style.right = 0;
        div.style.width = '5px';
        div.style.position = 'absolute';
        div.style.cursor = 'col-resize';
        div.style.userSelect = 'none';
        div.style.height = height + 'px';

        return div;
    }
}

function WriteOutout(text) {
    document.getElementById("Output").value += (text + '\r\n');
}