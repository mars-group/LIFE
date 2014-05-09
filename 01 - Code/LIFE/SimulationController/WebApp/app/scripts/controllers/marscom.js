'use strict';

angular.module('marsmissionControlApp')
    .controller('MarscomCtrl', function ($scope, $route, FilesFactory) {

        //Declaring File-Operations
        $scope.editor = {};
        $scope.dialogBox = {};
        $scope.updateConfrimBox ={};

        //Edit a saved file
        //This function is called from the edit-button in the fileListContainer-modal
        $scope.editFile = function (file){

            //Store ID of eddited file
            $scope.editor.editedFileId = file._id;

            //Init textarea with filecontens
            $scope.editor.text = file.content;

            //Display filename of eddited file
            $('#current-filename').html(file.filename);

            //Hide save-btn for new files
            //and display update-btn
            $('#save-btn').css('display', 'none');
            $('#update-btn').css('display', 'inline');

            $('#fileListContainer').modal('hide');
        };

        //Save File
        //The function is called from the modal 'enterFilenamePromt'
        $scope.saveFile = function(){

            // ##### START ##### -> If you update a saved file
            if($scope.editor.editedFileId !== undefined){
                //These two scope-vars are used in the modal
                //'updateConfrimBox'
                $scope.editor.updateFileId = $scope.editor.editedFileId;
                $scope.updateConfrimBox.message = "Update file ?";
                $('#updateConfrimBox').modal('show');

                return 1;
            }
            // ##### END ##### -> If you update a saved file

            // ##### START ##### -> If you have an new file
            $('#enterFilenamePromt').modal('hide');
            var filename = $.trim($scope.editor.chosenFilename);

            //Check for empty filename
            if(filename === ""){
                $scope.dialogBox.message = "The filename must not be empty";
                $('#dialogBox').modal('show');

                return 0;
            }

            //Check if chosen filename is already in use
            FilesFactory.fileExists(filename, function(data, status){
                if(data.fileExists){

                    //These two scope-vars are used in the modal
                    //'updateConfrimBox'
                    $scope.editor.updateFileId = data.fileId;
                    $scope.updateConfrimBox.message = "File '" + filename + "' already exists.<br/>Overwrite it ?";
                    $('#updateConfrimBox').modal('show');
                }
                else{
                    FilesFactory.createFile(filename, $scope.editor.text, function(file, status){
                        $scope.clearUpdateScope();
                        $scope.dialogBox.message = "File '" + filename + "' has been saved";

                        //After new file is created, show it in update-mode
                        $scope.editFile(file);

                        $('#dialogBox').modal('show');
                    });
                }
            });

            // ##### END ##### -> If you have an new file
        };

        //This function is called from the modal 'updateConfrimBox'
        $scope.updateFile = function (){
            FilesFactory.updateFile( $scope.editor.updateFileId, $scope.editor.text, function(data, status){
                $('#updateConfrimBox').modal('hide');
                $scope.dialogBox.message = "File has been updated";
                $('#dialogBox').modal('show');
            });
        };

        //This function is called form the button 'new file'
        $scope.clearUpdateScopeAndReload = function(){
            $scope.clearUpdateScope();
            $route.reload();
        }

        //It's saver to clear these three vars before the next use
        //of the updateConfrimBox
        $scope.clearUpdateScope = function (){
            delete $scope.editor.chosenFilename;
            delete $scope.editor.updateFileId;
            delete $scope.editor.editedFileId;
        };

        //Get filelist for the logged in user
        $scope.getFileList = function(){
            FilesFactory.getFiles(function(data, status){
                for(var i = 0; i < data.length; i++){
                    var split = data[i].createdAt.split('T');
                    data[i].createdAt = split[0];
                    split = data[i].updatedAt.split('T');
                    data[i].updatedAt = split[0];
                }

                $scope.savedFiles = data;
            });
        }

        $scope.deleteFile = function(file){

            if(window.confirm("Do you want to delete the file '" + file.filename + "'")){
                var index = $scope.savedFiles.indexOf(file);
                $scope.savedFiles.splice(index,1);
                FilesFactory.deleteFile(file._id, function(){
                    console.log("deleted file ", file);
                });
            }
        }

        //Initialize texteditor
        initTextEditor();
      });

function initTextEditor(){
    //Setting up jquery-linedtextarea plugin
    $('#editor').linedtextarea();

    //Insert TAB by TAB-Key and prevent losing focus on textarea
    $(document).delegate('#editor', 'keydown', function(e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 9) {
            e.preventDefault();
            var start = $(this).get(0).selectionStart;
            var end = $(this).get(0).selectionEnd;

            // set textarea value to: text before caret + tab + text after caret
            $(this).val($(this).val().substring(0, start)
                + "\t"
                + $(this).val().substring(end));

            // put caret at right position again
            $(this).get(0).selectionStart =
                $(this).get(0).selectionEnd = start + 1;
        }
    });
}