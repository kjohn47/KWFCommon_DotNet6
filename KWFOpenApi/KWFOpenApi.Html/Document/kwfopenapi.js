const DefaultSelectedMedia = "Json";
const DefaultSelectedStatusCode = "200";

//current state
var CurrentSelectedMetadata =
{
    ReqSelectedMedia: DefaultSelectedMedia,
    RespSelectedMedia: DefaultSelectedMedia,
    RespSelectedStatus: DefaultSelectedStatusCode,
    ReqMediaTypes: {},
    ReqSamples: {},
    RespSamples: {},
    ReqObjRef: {},
    EndpointId: undefined,
    EndpointRoute: undefined,
    EndpointMethod: undefined
}

//request box text states for all endpoints and media types
var LoadedRequests = {};

//show or hide endpoints from group
function ExpandEndpointGroup(group_div, endpoint_div_id) {
    let toggled = group_div.getAttribute("kwf-toggled");
    let groupDiv = document.getElementById(endpoint_div_id);
    if (toggled === "false") {
        group_div.setAttribute("kwf-toggled", "true");
        groupDiv.style.setProperty("display", "block");
        groupDiv.style.setProperty("visibility", "visible");
    }
    else {
        group_div.setAttribute("kwf-toggled", "false");
        groupDiv.style.setProperty("display", "none");
        groupDiv.style.setProperty("visibility", "hidden");
    }
}

function SelectEndpoint(endpoint_id) {
    var requestBox = document.getElementById("request_box");
    var reqRefBody = document.getElementById("req-obj-ref-item");
    var requestSelectedMediaSelect = document.getElementById("request_selected_media");
    //save previous state - CurrentSelectedMetadata should be previous endpoint
    SavePreviousRequestState(requestBox);

    //load endpoint to current state
    CurrentSelectedMetadata.EndpointId = endpoint_id;
    CurrentSelectedMetadata.EndpointRoute = document.getElementsByName("endpoint_route_" + endpoint_id)[0].getAttribute("value");
    CurrentSelectedMetadata.EndpointMethod = document.getElementsByName("endpoint_method_" + endpoint_id)[0].getAttribute("value");
    CurrentSelectedMetadata.ReqSamples = {};
    CurrentSelectedMetadata.ReqMediaTypes = {};
    CurrentSelectedMetadata.RespSamples = {};
    CurrentSelectedMetadata.ReqObjRef = {};

    //get req state for endpoint (if any)
    var currentRequest = LoadedRequests[endpoint_id];
    if (currentRequest === null || currentRequest === undefined) {
        CurrentSelectedMetadata.ReqSelectedMedia = DefaultSelectedMedia;
    }
    else {
        CurrentSelectedMetadata.ReqSelectedMedia = currentRequest.media;
    }

    var reqSamples = document.getElementsByName("request_sample_" + endpoint_id + "[]");
    //only map requests if conditions met
    if (CurrentSelectedMetadata.EndpointMethod !== "GET" && CurrentSelectedMetadata.EndpointMethod !== "DELETE") {
        if (reqSamples.length > 0) {
            //handle requests
            //save request samples to current state
            reqSamples.forEach(reqSample => {
                var sampleKey = reqSample.getAttribute("kwf-media-type");
                CurrentSelectedMetadata.ReqSamples[sampleKey] = reqSample.getAttribute("value");
                CurrentSelectedMetadata.ReqMediaTypes[sampleKey] = reqSample.getAttribute("kwf-media-type-name");
                CurrentSelectedMetadata.ReqObjRef[sampleKey] = reqSample.getAttribute("kwf-obj_ref");
            });

            //save first request from sample to history
            if (currentRequest === null || currentRequest === undefined) {
                currentRequest = {
                    body: {},
                    media: CurrentSelectedMetadata.ReqSelectedMedia
                };
                LoadedRequests[endpoint_id] = currentRequest;
            }

            if (currentRequest.body[CurrentSelectedMetadata.ReqSelectedMedia] === null || currentRequest.body[CurrentSelectedMetadata.ReqSelectedMedia] === undefined) {
                var sampleReqKey = DefaultSelectedMedia;
                var sampleRequest = CurrentSelectedMetadata.ReqSamples[sampleReqKey];

                if (sampleRequest === null || sampleRequest === undefined) {
                    sampleReqKey = Object.keys(CurrentSelectedMetadata.ReqSamples)[0];
                    sampleRequest = CurrentSelectedMetadata.ReqSamples[sampleReqKey];
                }

                LoadedRequests[endpoint_id].media = sampleReqKey;
                LoadedRequests[endpoint_id].body[sampleReqKey] = sampleRequest
            }
        }
        else {
            //save first request to history when no sample available
            CurrentSelectedMetadata.ReqMediaTypes[CurrentSelectedMetadata.ReqSelectedMedia] = "application/json"; //maybe should be plain text?
            if (currentRequest === null || currentRequest === undefined) {
                currentRequest = {
                    body: {},
                    media: CurrentSelectedMetadata.ReqSelectedMedia
                };
                LoadedRequests[endpoint_id] = currentRequest;
            }

            if (currentRequest.body[CurrentSelectedMetadata.ReqSelectedMedia] === null || currentRequest.body[CurrentSelectedMetadata.ReqSelectedMedia] === undefined) {
                var sampleReqKey = DefaultSelectedMedia;
                var sampleRequest = "\"\"";
                LoadedRequests[endpoint_id].media = sampleReqKey;
                LoadedRequests[endpoint_id].body[sampleReqKey] = sampleRequest
            }
        }

        //get req body for current media type selected
        //check if box is disable, enable it
        requestBox.removeAttribute("readonly");
        requestBox.classList.remove("textbox-readonly");
        requestBox.value = LoadedRequests[CurrentSelectedMetadata.EndpointId]?.body[CurrentSelectedMetadata.ReqSelectedMedia];
        reqRefBody.innerHTML = CurrentSelectedMetadata.ReqObjRef[CurrentSelectedMetadata.ReqSelectedMedia];
        reqRefBody.setAttribute("kwf-req-obj-ref", CurrentSelectedMetadata.ReqObjRef[CurrentSelectedMetadata.ReqSelectedMedia]);

        var mediaTypesAvailable = Object.keys(CurrentSelectedMetadata.ReqMediaTypes);
        mediaTypesAvailable.forEach(type => {
            var mediaOption = document.createElement("option");
            mediaOption.value = type;
            mediaOption.innerHTML = CurrentSelectedMetadata.ReqMediaTypes[type];
            mediaOption.selected = type == CurrentSelectedMetadata.ReqSelectedMedia;
            requestSelectedMediaSelect.append(mediaOption);
        })
    }
    //no object reference, no request sample, and no request, get and delete - no request body
    else {
        //maybe disable the box...
        requestBox.value = "";
        requestBox.classList.add("textbox-readonly");
        requestBox.setAttribute("readonly", true);
        requestSelectedMediaSelect.innerHTML = "";
        reqRefBody.innerHTML = "";
        reqRefBody.removeAttribute("kwf-req-obj-ref");
    }

    var endpointDataDiv = document.getElementById("api-selected-endpoint-data");
    endpointDataDiv.innerHTML = "[" + CurrentSelectedMetadata.EndpointMethod + "] => " + CurrentSelectedMetadata.EndpointRoute
}

function ReloadRequestSample() {
    if (CurrentSelectedMetadata !== null &&
        CurrentSelectedMetadata !== undefined &&
        CurrentSelectedMetadata.EndpointMethod !== null &&
        CurrentSelectedMetadata.EndpointMethod !== undefined &&
        CurrentSelectedMetadata.EndpointMethod !== "GET" &&
        CurrentSelectedMetadata.EndpointMethod !== "DELETE") {
            var requestBox = document.getElementById("request_box");
            requestBox.value = CurrentSelectedMetadata.ReqSamples[CurrentSelectedMetadata.ReqSelectedMedia];
    }
}

function ChangeReqMediaType(mediaTypeSelect) {
    var mediaType = mediaTypeSelect.value;

    //nothing selected or same media type, no change
    if (CurrentSelectedMetadata === null ||
        CurrentSelectedMetadata === undefined ||
        mediaType == CurrentSelectedMetadata.ReqSelectedMedia) {
            return;
    }

    //get and delete have no request body. no change
    if (CurrentSelectedMetadata.EndpointMethod === null ||
        CurrentSelectedMetadata.EndpointMethod === undefined ||
        CurrentSelectedMetadata.EndpointMethod === "GET" ||
        CurrentSelectedMetadata.EndpointMethod === "DELETE") {
            return;
    }

    var requestBox = document.getElementById("request_box");
    // save to request states
    SavePreviousRequestState(requestBox);

    CurrentSelectedMetadata.ReqSelectedMedia = mediaType;
    var requestsForEndpoint = LoadedRequests[CurrentSelectedMetadata.EndpointId];
    if (requestsForEndpoint === null || requestsForEndpoint === undefined) {
        LoadedRequests[CurrentSelectedMetadata.EndpointId] = {
            media: mediaType,
            body: {}
        }

        requestsForEndpoint = LoadedRequests[CurrentSelectedMetadata.EndpointId];
    }

    var stateForMediaType = requestsForEndpoint.body[mediaType];
    if (stateForMediaType === null || stateForMediaType === undefined) {
        var sampleReq = CurrentSelectedMetadata.ReqSamples[mediaType];

        if (sampleReq === null || sampleReq === undefined) {
            sampleReq = "\"\"";
        }

        LoadedRequests[CurrentSelectedMetadata.EndpointId].body[mediaType] = sampleReq;
    }

    reqRefBody.innerHTML = CurrentSelectedMetadata.ReqObjRef[mediaType];
    reqRefBody.setAttribute("kwf-req-obj-ref", CurrentSelectedMetadata.ReqObjRef[mediaType]);
    requestBox.removeAttribute("readonly");
    requestBox.classList.remove("textbox-readonly");
    requestBox.value = LoadedRequests[CurrentSelectedMetadata.EndpointId]?.body[mediaType];
}

function SavePreviousRequestState(requestBox) {
    if (CurrentSelectedMetadata.EndpointId !== null && CurrentSelectedMetadata.EndpointId !== undefined) {
        var prevReqState = LoadedRequests[CurrentSelectedMetadata.EndpointId];
        if (prevReqState !== null && prevReqState !== undefined) {
            LoadedRequests[CurrentSelectedMetadata.EndpointId].body[CurrentSelectedMetadata.ReqSelectedMedia] = requestBox.value;
            LoadedRequests[CurrentSelectedMetadata.EndpointId].media = CurrentSelectedMetadata.ReqSelectedMedia;
        }
    }
}