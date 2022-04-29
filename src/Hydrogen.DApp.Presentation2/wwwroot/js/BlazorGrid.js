function ConsoleWrite(data) {
    console.log(data);
}

function AlertWrite(data) {
    alert(data);
}

var MinimumColumWidth = 20;
var TableId = "";
var ColumnWidths = null;
var curColIndex, curCol;

function ResizableColumnTable(id, csharpInstance)
{
    TableId = id;
    var table = document.getElementById(TableId);

    if (table === null) return;

    CSharpInstance = csharpInstance;
    ResizableTable(table);
}

function ResizableTable(table) {
    console.log("ResizableTable");

    var row = table.getElementsByTagName('tr')[0],
        cols = row ? row.children : undefined;
    if (!cols) return;

    //console.log("Table Columns: " + cols.length.toString());

    ColumnWidths = new Array(cols.length);
    for (var i = 0; i < cols.length; i++) {
        ColumnWidths[i] = cols[i].clientWidth;
    }

    //console.log(ColumnWidths);

    table.style.overflow = 'hidden';
    var headerHeight = $(table).find('thead').height();

    for (var i = 0; i < cols.length; i++) {
        var div = createDiv(headerHeight);
        cols[i].appendChild(div);
        cols[i].style.position = 'relative';
        setListeners(div);
    }

    function setListeners(div) {
        div.addEventListener('mousedown', function (e) {
            curCol = e.target.parentElement;
            curColIndex = curCol.cellIndex;
        });

        div.addEventListener('mouseover', function (e) {
            e.target.style.borderRight = '5px solid #2c7ec9';
        })

        div.addEventListener('mouseout', function (e) {
            e.target.style.borderRight = '';
        })

        document.addEventListener('mousemove', function (e) {
            if (curCol) {
                var mouseX = e.pageX;
                var table = document.getElementById(TableId);
                var tableLeft = table.getBoundingClientRect().left;
                var inBetweenColumnsWidth = 0;

                for (var i = 0; i < curColIndex; i++) {
                    inBetweenColumnsWidth += ColumnWidths[i];
                }

                var newWidth = (mouseX - tableLeft) - inBetweenColumnsWidth;

                if (newWidth < MinimumColumWidth) newWidth = MinimumColumWidth;

                //console.log("mouseX: " + mouseX + " Table left: " + tableLeft + " inBetweenColumnsWidth: " + inBetweenColumnsWidth + " newWidth: " + newWidth + "curColIndex: " + curColIndex);

                // update the current column's width in the global ColumnWidths
                ColumnWidths[curColIndex] = newWidth;

                // set every row's cell's width
                for (var row = 0; row < table.rows.length; row++) {
                    for (var column = 0; column < ColumnWidths.length; column++) {
                        table.rows[row].cells[column].style.width = ColumnWidths[column] + 'px';
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
            curColIndex = undefined;
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