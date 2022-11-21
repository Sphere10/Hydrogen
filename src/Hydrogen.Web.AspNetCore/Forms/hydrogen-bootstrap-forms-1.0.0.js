/* 
 * hydrogen-bootstrap-forms-1.0.0.js
 *
 * Author: Herman Schoenfeld 2022
 * Version: 1.0.0
 * Copyright (c) 2022 Sphere 10 Software
 * 
 * This javascript library converts your standard HTML5 forms (styled with bootstrap) into AJAX. It depends on jQuery and Bootstrap.
 * 
 * To use this library:
 * 
 *  1. Ensure this script is loaded in the beginning of your HTML without deferrment (preferably header).
 *  
 *  2. For animated spinner on button, use the below Button instead of your usual form submit (replacing {formID} with your form's id) 
 *  
 *     <button class="btn btn-primary" type="submit" form="{formId}">
 *          <span id="{formId}_left_spinner" class="spinner-border spinner-border-sm me-2 invisible" role="status" aria-hidden="true"></span>
 *              {button text}
 *          <span id="{formId}_right_spinner" class="spinner-border spinner-border-sm ms-2 invisible" role="status" aria-hidden="true"></span>
 *     </button>
 *  
 *
 *  3. Append the below HTML after your form (replacing {formId})
 *  
 *      <div id="{formId}_result_marker" class="invisible"></div>
 *      <script language="javascript">
 *          F_Init('{formId}');
 *      </script>
 * 
 *  When form will POST to the destination, the "FormResult" response from server should be a JSON object formatted as follows:
 *  
 *   {
 *      "result" : true/false,
 *      "message: "message to display user",
 *      "type": "message" or "redirect" or "replace_page" or "replace_form"
 *      "url": "http://url.being.redirected.to",
 *      "content": "... HTML content which will replace the page or form ..."
 *	}
 * 
 */

function F_BeforeSubmit(formId, o) {
    // get form object
    var form = $('#' + formId);

    // set submit button to disabled and activate spiner
    var submit = form.find(':input[type="submit"]');
    if (!submit.prop('disabled')) {
        submit.prop('disabled', true);
        form.find('#' + formId + '_left_spinner').removeClass("invisible");
    }

    // remove prior validation errors
    form.find(".field-validation-error").remove();

    // clear the results
    $('#' + formId + '_result').replaceWith(' <div id = "' + formId + '_result' + '"></div>');

    // set the overlay (blocks input)
    form.find(".form-blocker-overlay").removeClass("invisible");
}


function F_Success(formId, result) {
    var form = $('#' + formId);

    switch (result.type) {
        case "redirect":
            location.assign(result.url);
            break;

        case "message":
            var alertType = result.result ? "success" : "danger";
            var alertHeader = result.result ? "Okay!" : "Apologies";
            var alertIcon = result.result ? "fa fa-check" : "fa fa-exclamation";
            F_ShowError(formId, alertType, alertIcon, alertHeader, result.message);
            break;

        case "replace_page":
            var doc = document;
            doc = doc.open("text/html");
            doc.write(result.content);
            doc.close();
            break;

        case "replace_form":
            form.replaceWith(result.content);
            if (result.message) {
                result.type = "message";
                F_Success(formId, result);
            }

            break;
    }

    if (result.result) {
        form[0].reset();
        form.find('input[type=text], textarea').val('');
    }
}

function F_Error(formId, status, error) {
    var alertType = "danger";
    var alertHeader = "Apologies";
    var alertIcon = "fa fa-exclamation";
    F_ShowError(formId, alertType, alertIcon, alertHeader, status + '-' + error);
}

function F_ShowError(formId, alertType, alertIcon, title, message) {
    var htmlToInject = '<div id="' + formId + '_result" class="form-result alert alert-dismissible alert-' + alertType + ' fade show"><strong><i class="' + alertIcon + '"></i> ' + title + '</strong> ' + message + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' + '</div>';
    var resultDiv = $('#' + formId + '_result');
    if (resultDiv.length > 0)
        resultDiv.replaceWith(htmlToInject);
    else
        $('#' + formId + '_result_marker').after(htmlToInject);
}

function F_Completed(formId) {
    var form = $('#' + formId);

    // remove overlay
    form.find(".form-blocker-overlay").addClass("invisible");

    // enable button
    var submit = form.find(':input[type="submit"]');
    submit.prop('disabled', false);

    // remove spinner
    form.find('#' + formId + '_left_spinner').addClass("invisible");

}

function F_Init(formId) {
    $(document).ready(function () {
        $('#' + formId).on("submit", function (event) {
            var formId = $(this).attr("id");
            event.preventDefault();
            var formValues = $(this).serialize();
            $.ajax({
                url: "/contact/form",
                type: "POST",
                data: formValues,
                cache: false,
                async: true,
                beforeSend: function (xhr) {
                    F_BeforeSubmit(formId, xhr);
                },
                success: function (data, status, xhr) {
                    F_Success(formId, data);
                },
                error: function (xhr, status, error) {
                    F_Error(formId, status, error)
                },
                complete: function (xhr, status) {
                    F_Completed(formId);
                }
            });
        });
    });
}