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
    form.find(".is-invalid").removeClass("is-invalid");
    form.find(".input-validation-error").removeClass("input-validation-error");

    // clear the results
    $('#' + formId + '_result').replaceWith(' <div id = "' + formId + '_result' + '"></div>');

    // set the overlay (blocks input)
    form.find(".form-blocker-overlay").removeClass("invisible");
}

function F_Success(formId, result) {
    var form = $('#' + formId);

    // Reset form (if applicable)
    if (result.result == true && form[0].options.clearOnSuccess == true)
        F_Reset(formId);

    switch (result.type) {
        case "redirect":
            location.assign(result.url);
            break;

        case "message":
            var alertType = result.result ? "success" : "danger";
            var alertHeader = result.result ? "" : "";
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
}

function F_Error(formId, status, error) {
    var alertType = "danger";
    var alertHeader = "";
    var alertIcon = "fa fa-exclamation";
    F_ShowError(formId, alertType, alertIcon, alertHeader, status + '-' + error);
}

function F_ShowError(formId, alertType, alertIcon, title, message) {
    var htmlToInject = '<div id="' + formId + '_result" class="form-result alert alert-dismissible alert-' + alertType + ' mb-0 mt-3 fade show"><strong><i class="' + alertIcon + '"></i> ' + title + '</strong> ' + message + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' + '</div>';
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

function F_Init(formId, options) {
    $(document).ready(function () {
        var form = $('#' + formId);

        // On first time initializing, we save a backup of the form for future resets
        if ($(window).data(formId) == null)
            $(window).data(formId, form.html())

        // set form options
        if (options == null)
            options = {
                clearOnSuccess: false,
                botProtect: false
            };
        form[0].options = options;

        // re-init choices for fetched form (choices.js)
        form[0].querySelectorAll('[sp10-choices]').forEach((toggle) => {
            const elementOptions = toggle.dataset.choices ? JSON.parse(toggle.dataset.choices) : {};

            const defaultOptions = {
                shouldSort: false,
                searchEnabled: false,
                classNames: {
                    containerInner: toggle.className,
                    input: 'form-control',
                    inputCloned: 'form-control-xs',
                    listDropdown: 'dropdown-menu',
                    itemChoice: 'dropdown-item',
                    activeState: 'show',
                    selectedState: 'active',
                },
            };

            const options = {
                ...elementOptions,
                ...defaultOptions,
            };

            var choices = $(toggle).data('choices');

            if (choices)
                choices.destroy();

            choices = new Choices(toggle, options);
            $(toggle).data('choices', choices);
        });

        // Init bootstrap tool-tips (WARNING: not sure if these are repeatable safe)
        form.find('[data-bs-toggle="tooltip"]').tooltip();

        // Init bootstrap pop-overs (WARNING: not sure if these are repeatable safe)
        form.find('[data-bs-toggle="popover"]').popover();


        // Add the AJAX handler
        form.on("submit", function (event) {
            // Increment submit counter
            var submitProp = form.find('input:hidden[name="SubmitCount"]');
            submitProp.val(parseInt(submitProp.val()) + 1);

            var formId = $(this).attr("id");
            event.preventDefault();
            var formValues = $(this).serialize();
            var action = $(this).attr("action");
            if (form[0].options.botProtect)
                action = atob(action);

            $.ajax({
                url: action,
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

function F_Finalize(formId) {
    var form = $('#' + formId);
    // destroy form event handlers
    form.off();
    // destroy choices objects
    form[0].querySelectorAll('[sp10-choices]').forEach((toggle) => {
        var choices = $(toggle).data('choices');
        if (choices)
            choices.destroy();
    });
}

function F_Reset(formId) {
    F_Finalize(formId);

    // Restore form from original backup
    var form = $('#' + formId);
    form.html($(window).data(formId)); // this restores the original backed-up form html which contains the javascript to call F_Init
}