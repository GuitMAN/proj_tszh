;
'use strict';

/* Controllers */



var HomeApp = angular.module('HomeApp', ['ui.bootstrap', 'ngAnimate', 'ngRoute', 'ngResource', 'ngCookies']);

ArrMonth = [
  { id: '1', name: 'Январь' },
  { id: '2', name: 'Февраль' },
  { id: '3', name: 'Март' },
  { id: '4', name: 'Апрель' },
  { id: '5', name: 'Май' },
  { id: '6', name: 'Июнь' },
  { id: '7', name: 'Июль' },
  { id: '8', name: 'Август' },
  { id: '8', name: 'Сентябрь' },
  { id: '10', name: 'Октябрь' },
  { id: '11', name: 'Ноябрь' },
  { id: '12', name: 'Декабрь' }
];

HomeApp.filter('JsonDate', function () {
    'use strict';
    return function (input) {
        if (input) {
            return parseInt(input.substr(6));
        }
        else {
            return;
        }
    };
});

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
     // $locationProvider.html5Mode(true);
      $routeProvide
          .when('/', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/login', {
              templateUrl: '/login/index',
              controller: 'LoginCtrl'
          })
          .when('/logoout', {
              templateUrl: '/login/logoout',
              controller: 'LogoutCtrl'
          })
          .when('/register', {
              templateUrl: '/login/register',
              controller: 'RegisterCtrl'
          })
          .when('/manage', {
              templateUrl: '/login/manage',
              controller: 'FeedbackCtrl'
          })
          .when('/feedback', {
              templateUrl: '/user/feedback',
              controller: 'FeedbackCtrl'
          })
          .when('/profile', {
              templateUrl: '/user/profile',
              controller: 'FeedbackCtrl'
          })
          .when('/editprof', {
              templateUrl: '/user/editprof',
              controller: 'EditProfCtrl'
          })
          .when('/meters', {
              templateUrl: '/user/ViewMeters',
              controller: 'MetersCtrl'
          })
          .when('/addmeter',{
              templateUrl: '/user/addmeter',
             controller: 'MetersCtrl'
          })
          .when('/datameters', {
              templateUrl: '/user/ViewDataMeters',
              controller: 'ViewDataMetersCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);




/* Factory */
HomeApp.factory('Article', [
  '$resource', function ($resource) {
      return $resource('/home/getarticle/:artId',
          {
              artId: 'articles'
          }, 
          {
              'query': { method: 'GET' }
          });    
  }
]);

/* Filter */
HomeApp.filter('checkmark', function () {
    return function (input) {
        return input ? '\u2713' : '\u2718';
    }
});

/* Filter */
HomeApp.filter('aspDate', function () {
    'use strict';
    return function (input) {
        if (input) {
            return parseInt(input.substr(6));
        }
        else {
            return;
        }
    };
});


/* Main page */
HomeApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;
      });
  }
]);




/*Section of authentication*/
HomeApp.controller('LoginInfoCtrl', function ($scope, $cookies, AUTH_EVENTS, AuthService, $rootScope, Session, $log)
{
    $scope.Session = Session;
    var curname = $cookies.get("username");
    var role = $cookies.get("userRole");
    var id = $cookies.get("userId");
    Session.create(1, id, role, curname);
    $scope.userRoles = role;
    $scope.isAuthorized = AuthService.isAuthorized;

    $scope.items = [
  'The first choice!',
  'And another choice for you.',
  'but wait! A third!'
    ];

    $scope.status = {
        isopen: false
    };

    $scope.toggled = function (open) {
        $log.log('Dropdown is now: ', open);
    };
    $scope.toggleDropdown = function($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };


    $scope.menu = function () {
        if (document.body.clientWidth < 768)
        {
            $scope.navCollapsed = !$scope.navCollapsed;
           // console.log($scope.navCollapsed);
            $scope.collapse = $scope.navCollapsed;
            if ($scope.navCollapsed) {
                $scope.navHeight = { height: '0' };
            } else {
                $scope.navHeight = { height: 'auto' };
            }
            //console.log("navHeght: ", $scope.navHeight);
        }
    }

   
});



/* for token verification  */
function getHttpConfig() {
    var token = angular.element("input[name='__RequestVerificationToken']").val();
    var config = {
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            '__RequestVerificationToken': token
        }
    };
    return config;
}

/*Login factory*/
HomeApp.factory('AuthService', function ($http, $cookies, Session) {
    return {
        login: function (credentials)
        {
 //           var config = getHttpConfig();
            return $http.post('/Login', credentials)//, config)
              .then(function (res)
              {             
                  Session.create(1, res.data.id, res.data.Role, res.data.Login);
                  return res;              
               })
              
        },
        register: function (reguser)
        {
            $http.post('/Login/Register', reguser)
                .then(function (res) {
                if (res.data[0] == 'Ok') {
                    Session.create(1, res.data.id, res.data.Role, res.data.Login);
                } else {
                    Session.destroy();
                }
                return data;
            })
        },
        logout: function () {
           return $http.post('/Login/logoout')//, config)
              .then(function (res) {
                  if (res.status == 200)
                  Session.destroy();
                 return res;
              })
        },
        manage: function (model) {
            return $http.post('/Login/manage', model)
                  .then(function (response) {                                           
                      return response;
                  })
        },
        isAuthenticated: function () {
            return Session.userId;
        },
        isAuthorized: function (authorizedRoles) {
            if (!angular.isArray(authorizedRoles)) {
                authorizedRoles = [authorizedRoles];
            }
            return (Session.userId &&
              authorizedRoles.indexOf(Session.userRole) !== -1);
        }
    }
});

/*service of global login data */
HomeApp.service('Session', function ($cookies) {
    this.create = function (sessionId, userId, userRoles, currentUser) {
        this.id = sessionId;
        this.userId = userId;
        this.currentUser = currentUser;
        this.userRoles= userRoles;
        this.status = status;
        $cookies.put('userId', userId);
        $cookies.put('username', currentUser);
        $cookies.put('userRole', userRoles)

    };
    this.destroy = function () {
        $cookies.remove('userId');
        $cookies.remove('username');
        $cookies.remove('userRole');
        this.id = null;
        this.userId = null;
        this.userRoles = null;
        this.status = null;
        this.currentUser = null;
    };
    return this;
})


/*fucking constant. I think remove that in future */
HomeApp.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})


HomeApp.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    editor: 'Moder',
    guest: 'User'
})

/* controller for login form*/
HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session, $location) {
    $scope.credentials = {
        username: '',
        password: '',
        RememberMe: false
    };
    $scope.response = '';
    $scope.login = function (credentials) {
        AuthService.login(credentials)
            .then(function (response)
            {             
                if (response.data[0] == 'Error') {
                    //$rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
                    $scope.response = response.data;

                } 
            
        });
    };
   
})


/* controller for logout*/
HomeApp.controller('LogoutCtrl', function ($http, AuthService, $location) {

    AuthService.logout();
    return  $location.path('#/');
})


/* For register form*/
HomeApp.controller('RegisterCtrl', function ($scope, $rootScope, $location, AuthService, Session) {
    $scope.reguser = {
        UserName: '',
        Password: '',
        ConfirmPassword: ''
    };
    $scope.Session = Session;
    if (AuthService.isAuthenticated())
    {
        return $location.path('#/');
    }
    $scope.register = function (reguser) {
        AuthService.register(reguser)
            .then(function (data) {
               
            });
        return $location.path('#/');
    };

})
/* controller for logout*/
HomeApp.controller('ManageCtrl', function ($scope, $http, AuthService, $location) {
    $scope.model = {
        NewPassword: '',
        ConfirmPassword: ''
    };

    $scope.status = false;
    $scope.manage = function (model) {
        AuthService.manage(model).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        })
        //return $location.path('#/');
    };
})


/* User`s controllers & services */
HomeApp.controller('FeedbackCtrl', function ($http, $scope, UserServices, Session) {

    $scope.feedmodel = {
        id: 0,
        id_uk: 0,
        id_user: Session.userId,
        title: '',
        message : ''       
    };

    $scope.status = false;
    $scope.submit = function (feedmodel) {
        UserServices.feedback(feedmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", data);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
    }
})

/* User`s controllers & services */
HomeApp.controller('EditProfCtrl', function ($http, $scope, UserServices, Session) {

    $scope.profmodel = {
        id: '',
        id_uk: '',
        UserId: '',
        Adress : '',
        Apartment: '',
        SurName: '',
        Name: '',
        Patronymic: '',
        Personal_Account: '',
        Email: '',
        phone: '',
        mobile:''
    };

    $scope.status = false;
    $scope.submit = function (profmodel) {
        UserServices.editprof(profmodel).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
        return $location.path('#/');
    }
});

//View all meters of user
HomeApp.controller('MetersCtrl', function ($http, $scope, UserServices, Session) {
    
    UserServices.viewmeters().then(function (response) {
        $scope.meters = response.data;
    });
});

//Add meter from user
HomeApp.controller('AddMeterCtrl', function ($scope, UserServices, Session) {

    $scope.meter = {
        UserId: Session.userId,
        Name: '',
        Serial: '',
        Status: false,
        Type: '',
        Measure: '',
        DateOfReview: null,
        firstdata: ''
    };
    ///////////////
    $scope.today = function () {
        $scope.DateOfReview = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.DateOfReview = null;
    };

    $scope.inlineOptions = {
        customClass: getDayClass,
        minDate: new Date(),
        showWeeks: true
    };

    $scope.dateOptions = {
        datepickerPopup: 'dd-MMMM-yyyy',
        //dateDisabled: disabled,
        formatYear: 'yy',
        maxDate: new Date(),
        minDate: new Date(1990,01,01),
        startingDay: 1
    };

    // Disable weekend selection
    function disabled(data) {
        var date = data.date,
          mode = data.mode;
        return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
    }

    $scope.toggleMin = function () {
        $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
        $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
    };

    $scope.toggleMin();

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };

    $scope.setDate = function (year, month, day) {
        $scope.dtDateOfReview = new Date(year, month, day);
    };

    //$scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    //$scope.format = $scope.formats[0];
    //$scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.popup2 = {
        opened: false
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
      {
          date: tomorrow,
          status: 'full'
      },
      {
          date: afterTomorrow,
          status: 'partially'
      }
    ];

    function getDayClass(data) {
        var date = data.date,
          mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }


    ///////////////


    $scope.submit = function (meter) {
        UserServices.addmeter(meter).then(function (response) {

            $scope.response = response.data;
        });
    }
});

//View all meters of user
HomeApp.controller('ViewDataMetersCtrl', function ($http, $scope, UserServices, Session) {
    $scope.data_ =
    {
        "id": '',
        "write": '',
        "data": '',
        "status": ''
    };
    $scope.submit = function (data_) {
        UserServices.addvaluemeter(data_).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
                $scope.datameters = $http.post('/User/ViewDataMeters');
            }
        });
    };
    UserServices.viewdatameters().then(function (response) {
        $scope.datameters = response.data;
    });

});

/* Factory of user`s controller */
HomeApp.factory('UserServices', function ($http) {
    return {
        feedback: function (feedmodel) {
            return $http.post('/User/Feedback', feedmodel)
              .then(function (response) {
                  return response;
            })
        },
        editprof: function (profmodel) {
            return $http.post('/User/editprof', profmodel)
              .then(function (response) {
                  return response;
            })
        },
        viewmeters: function(){
            return $http.post('/User/ViewMeters')
                .then(function (response) {
                          return response;
            })
        },
        addmeter: function (meter) {
            return $http.post('/User/AddMeter', meter)
                .then(function (response) {
                    return response;
                })
        },
        viewdatameters: function () {
            return $http.post('/User/ViewDataMeters')
                .then(function (response) {
                    return response;
                })
        },
        addvaluemeter: function (valuemeter) {
            return $http.post('/User/AddValueMeter', valuemeter)
                .then(function (response) {
                    return response;
                })
        }
    }
});



