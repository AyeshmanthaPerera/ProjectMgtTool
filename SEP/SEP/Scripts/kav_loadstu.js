
$(document).ready(function () {

    var $count = 0;

    ///*************************************************************
    //Feeding the selected users informtions to the central grid
    //**************************************************************
    $('#ev1 li a').click(function () {
        $("#ev1 li").removeClass('Clicked');
        $(this).parent().addClass('Clicked');
        console.log("HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
        var $selectedUserName = $("#ev1 li.Clicked a").html();
        console.log("selected usrrrrrrrrrrrrrrrrr" + $selectedUserName);

        $.ajax({
            url: "/Students_module/GetStudent",
            data: { selectedUser: $selectedUserName },
            type: "POST",
            dataType: "json",
            success: function (data) {

                $("#innerImg").attr("src", data.student.Avatar);
                $("#tname").html(data.student.Name);
                $("#lReg").html(data.student.RegistrationNo);
                $("#tgpa").html(data.student.CGPA);
                $("#tphone").html(data.student.ContactNo);
                $("#temail").html(data.student.Email);
            },
            error: function () {
                alert("Failed");
            }
        });
    });


    ///**********************************************
    //Adding current logged in user to his own group
    //***********************************************
    $('#btnCreate').click(function () {
        $("#btnCreate").removeClass('Clicked');
        $(this).parent().addClass('Clicked');

        var $UserId = $("#myReg").html();

        var $moduleId = $("#sessionModule").html();

        $.ajax({
            url: "/Students_module/FillMe",

            data: { loggedUserID: $UserId, currentModule: $moduleId },
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data == 2) {
                    alert("You already have a group and you can't create a new one!");
                }
                else {
                    alert("You're created a group successfully now add members to your group!!!");
                }
            },
            error: function () {
                alert("Failed in inserting loged in user to the table");
            }
        });

        $("#btnCreate").prop("disabled", true);
    });




    //***********************************************
    //Adding members to my group
    //***********************************************

    $("#addGrp").click(function () {

        var $reg = $("#lReg").html();
        console.log("leg issssssssss" + $reg);
        var $module = $("#sessionModule").html();
        var $leaderReg = $("#myReg").html();

        if ($reg.length > 3) {
            $.ajax({
                url: "/Students_module/addToGroup",
                data: { studentId: $reg, module: $module, leaderID: $leaderReg },
                type: "POST",
                dataType: "json",
                success: function (data) {

                    if (data == 1) {
                        alert($reg + " has been successfully added to your group!!!");

                        $.ajax({
                            url: "/Students_module/FillTable",
                            data: { studentId: $reg },
                            type: "POST",
                            dataType: "json",
                            success: function (data) {
                                var currentStatus = "not clear";
                                if (data.acceptanceStatus.status == 0) {
                                    currentStatus = "Pending";
                                }
                                else if (data.acceptanceStatus.status == 1) {
                                    currentStatus = "Accepted";
                                }
                                else {
                                    currentStatus = "Rejected";
                                }
                                $("<tr><td><img id='imgIcon' src='" + (data.student.Avatar) + "'/></td><td>" + data.student.Name + "</td><td>" + currentStatus + "</td></tr>").appendTo("#Grptable");

                            },
                            error: function () {
                                alert("Failed to fill the table !");
                            }
                        });

                        $.ajax({
                            url: "/Students_module/UpdateNotifications",
                            data: { newMemberId: $reg },
                            type: "POST",
                            dataType: "json",
                            success: function (data) {
                                if (data == 1) {
                                    alert("Send group request for " + $reg + " !");
                                }
                                else {
                                    alert("Couldn't send the group request");
                                }
                            },
                            error: function () {
                                alert("Failed to update in to notification table");
                            }
                        });
                    }
                    else if (data == 3) {
                        alert("Please create a group before you add members!!");
                    }

                    else if (data == 4) {
                        alert("This member is already in your group, Please insert a new member");
                    }

                    else {
                        alert(" You can't add more members to the group!!!");
                    }
                },
                error: function () {
                    alert("Error occure while adding members to your group");
                }
            });


        }
        else {
            alert("Please select a student to add to the group!");
        }

    });

    //final close

});






