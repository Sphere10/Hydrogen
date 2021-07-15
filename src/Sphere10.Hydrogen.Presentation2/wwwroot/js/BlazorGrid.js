function ConsoleWrite(data) {
    console.log(data);
}

function AlertWrite(data) {
    alert(data);
}

var MinimumColumWidth = 10;
var TableId = "";
//var Table = null;
var CSharpInstance = null;
function ResizableColumnTable(id, csharpInstance)
{
    TableId = id;
    var table = document.getElementById(TableId);

    if (table === null) return;

    CSharpInstance = csharpInstance;
    ResizableTable(table);
}

function ResizableTable(table) {
    var row = table.getElementsByTagName('tr')[0],
        cols = row ? row.children : undefined;
    if (!cols) return;

    table.style.overflow = 'hidden';
    var headerHeight = $(table).find('thead').height();

    for (var i = 0; i < cols.length; i++) {
        var div = createDiv(headerHeight);
        cols[i].appendChild(div);
        cols[i].style.position = 'relative';
        setListeners(div);
    }

    function setListeners(div) {
        var pageX, curCol, nxtCol, curColWidth, nxtColWidth;
        var curColIndex, nextColIndex, newWidth, curColTitle;

        div.addEventListener('mousedown', function (e) {
            curCol = e.target.parentElement;

            curColTitle = curCol.childNodes[1].wholeText;
            nxtCol = curCol.nextElementSibling;

            curColIndex = curCol.cellIndex;
            nextColIndex = curColIndex + 1;

            pageX = e.pageX;

            var padding = paddingDiff(curCol);

            curColWidth = curCol.offsetWidth - padding;
            if (nxtCol)
                nxtColWidth = nxtCol.offsetWidth - padding;
        });

        div.addEventListener('mouseover', function (e) {
            e.target.style.borderRight = '5px solid #2c7ec9';
        })

        div.addEventListener('mouseout', function (e) {
            e.target.style.borderRight = '';
        })

        document.addEventListener('mousemove', function (e)
        {
            if (curCol)
            {
                var mouseX = e.pageX;
                var table = document.getElementById(TableId);
                var tableLeft = table.getBoundingClientRect().left;
                var inBetweenColumnsWidth = 0;
                if (curColIndex > 0) { inBetweenColumnsWidth = table.rows[0].cells[curColIndex - 1].offsetWidth; }

                var newWidth = (mouseX - tableLeft) - inBetweenColumnsWidth;
                if (newWidth < MinimumColumWidth) newWidth = MinimumColumWidth;

console.log("mouseX: " + mouseX + " Table left: " + tableLeft + " inBetweenColumnsWidth: " + inBetweenColumnsWidth + " newWidth: " + newWidth);

                // set every row's cell's width
                for (var i = 0; row = table.rows[i]; i++)
                {
                    table.rows[i].cells[curColIndex].style.width = newWidth + 'px';
                }

                curColWidth = newWidth;
            }
        });

        document.addEventListener('mouseup', function (e) {
            if (curColIndex === undefined) return;

            // call a static C# function 
            //DotNet.invokeMethodAsync("SystemX", "SaveColumnWidth", curColTitle, newWidth);

            // call an instance C# function
            // CSharpInstance.invokeMethodAsync("SaveColumnWidth", curColTitle, newWidth);

            newWidth = undefined;
            curCol = undefined;
            nxtCol = undefined;
            pageX = undefined;
            nxtColWidth = undefined;
            curColWidth = undefined;
            curColIndex = undefined;
            nextColIndex = undefined;
            curColTitle = undefined;
        });
    }

    function createDiv(height) {
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

    function paddingDiff(col) {
        if (getStyleVal(col, 'box-sizing') == 'border-box') {
            return 0;
        }

        var padLeft = getStyleVal(col, 'padding-left');
        var padRight = getStyleVal(col, 'padding-right');

        return (parseInt(padLeft) + parseInt(padRight));
    }

    function getStyleVal(elm, css) {
        return (window.getComputedStyle(elm, null).getPropertyValue(css));
    }
}