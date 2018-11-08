$(document).ready(function () {
    $.ajax({
        url: "http://todoapiswin.azurewebsites.net/api/Todo?apiKey=work",
        method: "GET",
        success: function (data) {
            data.forEach(element => {
                addElement(element);
            });
        }
    })
    $("#newTaskForm").submit((formEvent) => {
        formEvent.preventDefault();
        var taskApiKey = $("#apiKeyInput").val();
        var taskTitle = $("#taskTitleInput").val();
        $.ajax({
            url: "http://todoapiswin.azurewebsites.net/api/Todo?apiKey=" + taskApiKey,
            method: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                task: taskTitle
            }),
            success: function (data) {
                addElement(data);
            }
        })
    });
});

function addElement(element) {
    $("#taskList").append("<li>" + element.task + "</li>");
}