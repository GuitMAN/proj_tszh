/* User`s controllers & services */
HomeApp.controller('ReadFeedBackCtrl', function ($http, $scope, UserServices, Session) {

    $scope.feedmodel = {
        id: 0,
        id_uk: 0,
        id_user: Session.userId,
        title: '',
        message: ''
    };

})



HomeApp.controller('ProfileCtrl', function ($scope){

})

/* User`s controllers & services */
HomeApp.controller('EditOperProfileCtrl', function ($http, $scope, OperServices, Session, $location) {

    //$scope.profmodel = {
    //    id_uk: '',
    //    UserId: '',
    //    Adress: '',
    //    Apartment: '',
    //    SurName: '',
    //    Name: '',
    //    Patronymic: '',
    //    Personal_Account: '',
    //    Email: '',
    //    phone: '',
    //    mobile: ''
    //};

    $scope.status = false;
    $scope.submit = function (profmodel) {
        OperServices.editprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
        return $location.path('#/');
    }
});

/* User`s controllers & services */
HomeApp.controller('CreateOperProfCtrl', function ($http, $scope, OperServices, Session, $location) {

    $scope.profmodel = {
        id_uk: '',
        AdmtszhId: Session.userId,
        SurName: '',
        Name: '',
        Patronymic: '',
        post: ''
    };

    $scope.status = false;
    $scope.submit = function (profmodel) {
        OperServices.createprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
                return $location.path('#/');
            }
        });
        
    }
});

HomeApp.controller('ViewUserCountersCtrl', function ($http, $scope, OperServices, Session, $location) {
    month_year = {
        month: '',
        year: ''
    };

    OperServices.viewcounters()

  });

/* Factory of user`s controller */
HomeApp.factory('OperServices', function ($http) {
    return {
        feedback: function (feedmodel) {
            return $http.post(_host + '/Admtszh/postfeed', feedmodel)
              .then(function (response) {
                  return response;
              })
        },
        editprof: function (profmodel) {
            return $http.post(_host + '/Admtszh/editprof', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        createprof: function (profmodel) {
            return $http.post(_host + '/Admtszh/editprof', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        viewcounters: function (profmodel) {
            return $http.post(_host + '/Admtszh/editprof', month_year)
              .then(function (response) {
                  return response;
              })
        }
 
    }
});


