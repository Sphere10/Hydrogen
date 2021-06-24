window.dispatchContentLoadedEvent = () => window.document.dispatchEvent(new Event("DOMContentLoaded", {
    bubbles: true,
    cancelable: true
}));

window.showModal = () => {
    $("#modal").modal({
        backdrop: "static",
        keyboard: false
    });
}

window.hideModal = () => {
    $("#modal").modal('hide')
}

window.initDataTableById = (id, options) => {
    $('#' + id).DataTable(options);
}

window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text)
            .catch(function (error) {
                alert(error);
            });
    }
};

document.addEventListener("DOMContentLoaded", () => {
    let input = $('.search-input');

    input.keyup(() => {
        if (input.val().length === 0)
        {
            $('.search-input-results').removeClass('show');
        }
        else
        {
            $('.search-input-results').addClass('show');
        }
    });

    input.blur(() => {
        $('.search-input-results').removeClass('show');
    });
});




// 

function ConsoleWrite(data)
{
    alert("Hello");

    console.log(data);
}

var MinimumColumWidth = 80;
var Table = null;
var CSharpInstance = null;
function ResizableColumnTable(id, csharpInstance) {
    Table = document.getElementById(id);
    CSharpInstance = csharpInstance;
    ResizableTable(Table);
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

        document.addEventListener('mousemove', function (e) {
            if (curCol) {
                var diffX = e.pageX - pageX - 24;
                newWidth = curColWidth + diffX;

                if (newWidth < MinimumColumWidth) newWidth = MinimumColumWidth;

                //console.log('mouse move: Column: ' + curColIndex.toString() + ' width: ' + newWidth.toString());

                //nxtCol.style.width = (nxtColWidth - diffX) + 'px';

                // set every row's cell's width
                for (var i = 0; row = Table.rows[i]; i++) {
                    Table.rows[i].cells[curColIndex].style.width = newWidth + 'px';
                }

                ////set the header column width
                //curCol.style.width = (curColWidth + diffX) + 'px';
                //
                ////set every row's cell's width
                //for (var i = 0; row = WorkAssignmentsTable.rows[i]; i++)
                //{
                //    WorkAssignmentsTable.rows[i].cells[curColIndex - 1].style.width = newWidth + 'px';
                //}
            }
        });

        document.addEventListener('mouseup', function (e) {
            if (curColIndex === undefined) return;

            // call a static C# function 
            //DotNet.invokeMethodAsync("SystemX", "SaveColumnWidth", curColTitle, newWidth);

            // call an instance C# function
//            CSharpInstance.invokeMethodAsync("SaveColumnWidth", curColTitle, newWidth);

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
        return (window.getComputedStyle(elm, null).getPropertyValue(css))
    }
}


