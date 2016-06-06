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


HomeApp.controller('OperProfileCtrl', function () {
});

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

HomeApp.controller('ViewUserCountersCtrl', function ($http, $scope, OperServices, Session, $location, $routeParams) {
    $scope.month_year = {
        month: '',
        year: ''
    };
    $scope.status = false;

    var now;
    $scope.now_y = false;
    if (now==null) now= new Date();
    if (!$routeParams.year) {
        $scope.month_year.year = now.getFullYear();
        $routeParams.year = now.getFullYear();
        $scope.month_year.month = now.getMonth();
    }
    else {
        if (parseInt($routeParams.year) > now.getFullYear()) {
            $scope.month_year.year = now.getFullYear();
            $routeParams.year = now.getFullYear();
            $scope.month_year.month = now.getMonth();

        }
        else {
            $scope.month_year.year = parseInt($routeParams.year);
            if ($routeParams.month) {
                $scope.month_year.month = parseInt($routeParams.month);
            }
            else {
                $scope.month_year.month = now.getMonth();
            }
        }
    }

    $scope.NameMonth = ArrMonth[$scope.month_year.month].name;

    $scope.monthOptions = [];
    for (var i = 0; i < 12; ++i) {
        $scope.monthOptions[i] = ArrMonth[i];
        if ($scope.month_year.year == now.getFullYear()) {
            if (i == now.getMonth()) {
                break;
            }
        }
    };
    OperServices.viewcounters($scope.month_year).then(function (response) {
        $scope.counter = response.data;
        console.log("data:", response);
        if (response.data[0] == 'Ok') {
            $scope.status = true;
        }
    });
    $scope.down_y = function () {
        $scope.now_y = true;
        $scope.month_year.year = $scope.month_year.year - 1;
        $scope.monthOptions = [];
        for (var i = 0; i < 12; ++i) {
            $scope.monthOptions[i] = ArrMonth[i];
            if ($scope.month_year.year == now.getFullYear()) {
                if (i == now.getMonth()) {
                    break;
                }
            }
        };
    }
    $scope.up_y = function () {
        if ($scope.month_year.year >= now.getYear()) {
            $scope.now_y = true;
        } else {
            $scope.now_y
        }
        $scope.month_year.year = $scope.month_year.year + 1;
        $scope.monthOptions = [];
        for (var i = 0; i < 12; ++i) {
            $scope.monthOptions[i] = ArrMonth[i];
            if ($scope.month_year.year == now.getFullYear()) {
                if (i == now.getMonth()) {
                    break;
                }
            }
        };
    }

    $scope.select_month = function () {    
        $scope.month_year.month = $scope.selectedMonth;
        OperServices.viewcounters($scope.month_year).then(function (response) {
            $scope.counter = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
    }

  });

/* Factory of user`s controller */
HomeApp.factory('OperServices', function ($http) {
    return {
        feedback: function (feedmodel) {
            return $http.post(_host + '/admtszh/postfeed', feedmodel)
              .then(function (response) {
                  return response;
              })
        },
        editprof: function (profmodel) {
            return $http.put(_host + '/admtszh/editprof', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        createprof: function (profmodel) {
            return $http.post(_host + '/admtszh/editprof', profmodel)
              .then(function (response) {
                  return response;
              })
        },
        viewcounters: function (month_year) {
            return $http.post(_host + '/admtszh/viewcounters', month_year)
              .then(function (response) {
                  return response;
              })
        }
 
    }
});


