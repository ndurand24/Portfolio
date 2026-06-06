/**********************************************************************************
 *   Program        : register.js
 *   Description    : Service layer for MTG register page
 *   Author         : Noah Durand
 *   Date Created   : 2026-06-05
 *   Last Rev. Date : 2026-06-05
 **********************************************************************************/


// URL for APS local server (Subject to change on live deployment)
let url = "https://localhost:7101";

// Variable to check password match
let samePass = false;

// Variable to check Username availability
let sameName = false;

/******************************************************************************
    Function    : - (page load)
    Description : Assigns callback functions to page elements
    Returns     : Nothing
******************************************************************************/
$(() => {
    // Call UserRegister() any time register button is clicked
    $("[name=register]").on("click", function(){
        UserRegister();
    })

    // Bring user back to login page if clicked
    $("[name=backToLogin]").on("click", function(){
        window.location.href = "../Login/login.html";
    })

    // Check if username is available on text change
    $("[name=username]").on("change", function(){
        UserTaken();
    })

    // Anytime the user changes the password verification text, check if both text boxes are equal (passwords match)
    $("[name=repassword").on("change", function(){
        PasswordMatch();
    })
})

/******************************************************************************
    Function    : PasswordMatch()
    Description : Checks if new password and password re-entry match
    Returns     : Nothing
******************************************************************************/
function PasswordMatch()
{
    if ($("[name=password]").val() == $("[name=repassword]").val())
    {
        samePass = true;
        $("#responseText").text("Passwords match").css("color", "green");
    }
    else
    {
        samePass = false;
        $("#responseText").text("Passwords do not match.").css("color", "red");
    }
}
 
/******************************************************************************
    Function    : UserTaker()
    Description : Runs GET request to check if username is already taken
    Returns     : Nothing
******************************************************************************/
function UserTaken()
{
    let username = $("[name=username]").val();
    MakeAjaxCall(url + "/checkuser?username="+ encodeURIComponent(username), "GET", {}, "json", UserAvailable, AjaxError);
}

/******************************************************************************
    Function    : UserAvailable()
    Description : Success handler for UserTaken() ajax call. Makes status text
                  show available and sets sameName variable to true
    Parameters  : jsonData - data sent back from server
    Returns     : Nothing
******************************************************************************/
function UserAvailable(jsonData)
{
    $("#responseText").text("Username available!").css("color", "green");
    sameName = true;
}

/******************************************************************************
    Function    : UserRegister()
    Description : If samePass and sameName are true, a POST request is sent
                  to server to register new user data
    Returns     : Nothing
******************************************************************************/
function UserRegister()
{
    // Check if each field is filled, show error and return if not
    if ($("[name=username]").val() == "" || $("[name=password]").val() == "" || $("[name=email]").val() == "")
    {
        $("#responseText").text("Missing one or more pieces of user data.").css("color", "red");
        return;
    }
        
    // If user passwords match & name is available, allow registration
    if (samePass && sameName)
    {
        let user = {};
        user.username = $("[name=username]").val();
        user.password = $("[name=password]").val();
        user.email = $("[name=email]").val();
        MakeAjaxCall(url + "/register", "POST", user, "json", RegisterSuccess, AjaxError);
    }
}

/******************************************************************************
    Function    : RegisterSuccess()
    Description : Success handler for UserRegister() ajax call. Redirects to
                  login page
    Parameters  : jsonData - data sent back from server
    Returns     : Nothing
******************************************************************************/
function RegisterSuccess(jsonData)
{
    console.log(jsonData.status);
    window.location.replace("../Login/login.html");
}

/******************************************************************************
    Function    : MakeAjaxCall()
    Description : Ajax call function
    Parameters  : serverURL - server URL
                  reqMethod - request method (GET, POST, PUT, DELETE)
                  data - data to send to server
                  serverResp - data type of server response (JSON, HTML)
                  successCallback - Ajax success callback handler (function)
                  errorCallback - Ajax fail callback handler (function)
    Returns     : Nothing
******************************************************************************/
function MakeAjaxCall(serverURL, reqMethod, data, serverResp, successCallback, errorCallback)
{
    let ajaxData = {};
    ajaxData.url = serverURL;
    ajaxData.type = reqMethod;
    ajaxData.data = JSON.stringify(data);
    ajaxData.dataType = serverResp;
    ajaxData.success = successCallback;
    ajaxData.error = errorCallback;
    ajaxData.contentType = "application/json";

    $.ajax(ajaxData);
}

/******************************************************************************
    Function    : AjaxError()
    Description : General Ajax fail function
    Parameters  : xhr - http server response code
                  errorStatus - status of error
                  errorThrown - type of error thrown
    Returns     : Nothing
******************************************************************************/
function AjaxError(xhr, errorStatus, errorThrown)
{
    let statusCode = xhr.status;

    let response = xhr.responseJSON;

    switch(statusCode)
    {
        case 400: // Server sent bad request code
            $("#responseText").text("Bad request: " + response.status).css("color", "red");
            break;

        case 401: // Server sent unauthorized code
            $("#responseText").text("Incorrect username or password.").css("color", "red");
            break;

        case 409: // Server sent conflict code
            $("#responseText").text("Username already taken!").css("color", "red");
            sameName = false;
            break;

        case 500: // Server sent internal error code
            $("#responseText").text("Server Error. Please try again later.").css("color", "red");
            break;

        default: // Unexpected error
            $("#responseText").text("An unexpected error has occured").css("color", "red");
            break;
    }
}