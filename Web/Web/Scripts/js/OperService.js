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

    $scope.modalShown = false;
    $scope.toggleModal = function () {
        $scope.modalShown = !$scope.modalShown;
    };
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
            if (response.data[0] === 'Ok') {
                $scope.status = true;
            }
        });
        return $location.path('#/');
    }
});


/* User`s controllers & services */
HomeApp.controller('NewOperProfileCtrl', function ($http, $scope, OperServices, Session, $location) {

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
        OperServices.createprof(profmodel).then(function (response) {
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

    //$scope.profmodel = {
    //    id_uk: '',
    //    AdmtszhId: Session.userId,
    //    SurName: '',
    //    Name: '',
    //    Patronymic: '',
    //    post: ''
    //};

    $scope.status = false;
    $scope.submit = function (profmodel) {
        OperServices.createprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] === 'Ok') {
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
    $scope.now_y = true;
    if (now===null) now= new Date();
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
        if ($scope.month_year.year === now.getFullYear()) {
            if (i === now.getMonth()) {
                break;
            }
        }
    };
    OperServices.viewcounters($scope.month_year).then(function (response) {
        $scope.counter = response.data;
        console.log("data:", response);
        if (response.data[0] === 'Ok') {
            $scope.status = true;
        }
    });
    $scope.down_y = function () {
        $scope.now_y = false;
        $scope.month_year.year = $scope.month_year.year - 1;
        $scope.monthOptions = [];
        for (var i = 0; i < 12; ++i) {
            $scope.monthOptions[i] = ArrMonth[i];
            if ($scope.month_year.year === now.getFullYear()) {
                if (i === now.getMonth()) {
                    break;
                }
            }
        };
    }
    $scope.up_y = function () {
        if ($scope.month_year.year >= now.getFullYear()) {
            $scope.now_y = true;
        }
        else
        {
            $scope.month_year.year = $scope.month_year.year + 1;
        }
        $scope.monthOptions = [];
        for (var i = 0; i < 12; ++i) {
            $scope.monthOptions[i] = ArrMonth[i];
            if ($scope.month_year.year === now.getFullYear()) {
                if (i === now.getMonth()) {
                    break;
                }
            }
        }

    }

    $scope.select_month = function () {    
        $scope.month_year.month = $scope.selectedMonth;
        OperServices.viewcounters($scope.month_year).then(function (response) {
            $scope.counter = response.data;
            console.log("data:", response);
            if (response.data[0] === 'Ok') {
                $scope.status = true;
            }
        });
    }

  });


HomeApp.controller('ViewUsersCtrl', function ($scope) {

})

HomeApp.controller('SettingsCtrl', function ($scope, OperServices, $http, $sce) {
    $scope.model = {
        id: ''
    };

    $scope.submit = function(model) {
        OperServices.setsettings($scope.model).then(function (response) {
            $scope.resp = response.data;
        });
    };
    OperServices.viewsettings().then(function (response) {
        $scope.model = response.data;
   
        $http.get(_host + '/admin/ViewAddrUk', { params: { id: $scope.model.id } }).then(function (response) {
            $scope.sce = $sce;
            $scope.t = response.data;
        })
    });

    OperServices.viewaddres().then(function (response)
    {
        $scope.addresses = response.data;
    })

})

HomeApp.controller('EditUsersCtrl', function ($scope, OperServices, $routeParams, $sce) {
    OperServices.viewuseredit($routeParams['id']).then(function (response) {
        $scope.profmodel = response.data;
    });

    $scope.submit = function (profmodel) {
        OperServices.saveuseredit(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] === 'Ok') {
                $scope.status = true;
              //  return $location.path('#/');
            }
        });

    }

})

HomeApp.controller('ArticlesCtrl', function (){
});

HomeApp.controller('EditArticleCtrl', function ($scope, OperServices, $routeParams, $sce) {
    OperServices.editarticle_get($routeParams['id']).then(function (response) {
        $scope.article = response.data;
    });

    $scope.submit = function (article) {
        OperServices.editarticle_put(article).then(function (response)
        {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data === 'Ok') {
                $scope.status = true;
                return $location.path('#/articles');

            }

        })
    }

})



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
        },
        viewuseredit: function (id) {
            return $http.get(_host + '/admtszh/edituser', { params: { id: id } }).then(function (response) {
                return response;
            })
        },
        saveuseredit: function (usermodel) {
            return $http.put(_host + '/admtszh/edituser', usermodel)
                .then(function (response) {
                    return response;
                })
        },
        viewsettings: function () {
            return $http.get(_host + '/admtszh/edituk').then(function (response) {
                return response;
            })
        },
        setsettings: function (uk_model) {
            return $http.put(_host + '/admtszh/edituk', uk_model).then(function (response) {
                return response;
            })
        },
        viewaddres: function () {
            return $http.get(_host + '/admtszh/viewaddruk').then(function (response) {
                return response;
            })
        },
        editarticle_get: function (id) {
            return $http.get(_host + '/admtszh/editarticle', { params: { id: id } }).then(function (response) {
                return response;
            })
        },
        editarticle_put: function (article) {
            return $http.put(_host + '/admtszh/editarticle', article).then(function (response) {
                return response;
            })
        },

    }
        
});

HomeApp.directive('modalDialog', function () {
    return {
        restrict: 'E',
        scope: {
            show: '='
        },
        replace: true, // Замените на шаблон
        transclude: true, // мы хотим вставлять пользовательский контент внутри директивы
        link: function (scope, element, attrs) {
            scope.dialogStyle = {};

            if (attrs.width) {
                scope.dialogStyle.width = attrs.width;
            }

            if (attrs.height) {
                scope.dialogStyle.height = attrs.height;
            }

            scope.hideModal = function () {
                scope.show = false;
            };
        },
        templateUrl: _host + '/admtszh/editprof'
        // Смотрите ниже
    };
});
