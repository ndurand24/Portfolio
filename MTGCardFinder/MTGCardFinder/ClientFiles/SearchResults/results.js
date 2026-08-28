/**********************************************************************************
 *   Program        : search.js
 *   Description    : Service layer for MTG search page
 *   Author         : Noah Durand
 *   Date Created   : 2026-06-05
 *   Last Rev. Date : 2026-06-10
 **********************************************************************************/


// URL for ASP local server (Subject to change on live deployment)
let url = "https://localhost:7101"

/******************************************************************************
    Function    : - (page load)
    Description : Assigns callback functions to page elements
    Returns     : Nothing
******************************************************************************/

$(() => {
    let searchData = {};

    Object.keys(sessionStorage).forEach(function(key){
        searchData[key] = sessionStorage.getItem(key);
    })

    MakeAjaxCall(url + "/search", "GET", searchData, "json", SearchSuccess, AjaxError);
})

function SearchSuccess(ajaxData)
{
    console.log(ajaxData);

    let table = "<table><tr><th>Image</th><th>Name</th></tr>";

    ajaxData.forEach(element => {
        table += "<tr><td><img src=\"" + element.imageUrl + "\" alt=\"Error Loading Image\"></td><td>" + element.name + "</td>";
    });

    $("#resultsTable").html(table);
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
    ajaxData.xhrFields = {
        withCredentials: true
    };

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