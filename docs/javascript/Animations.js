//********************************************************************************
//
//  Animations for elements common throughout the site
//  Andrew Gray
//  Aug 17, 2019
//
//********************************************************************************

// Shows the navigation menu on menu button click
var isNavVisible = false
function showNav() {
  $("#firstbar").toggleClass('navVisible2')
  $("#thirdbar").toggleClass('navVisible1')
  if (isNavVisible) {
    setTimeout(() => {
      $("#secondbar").css({
        'opacity': '1'
      })
    }, 150)
    $("#nav-container").animate({
      left: -200
    }, 500)
  } else {
    $("#secondbar").css({
      'opacity': '0'
    })
    $("#nav-container").animate({
      left: 0
    }, 500)
  }
  isNavVisible = !isNavVisible
}

// Performs sldiing-up animation on page load
function loadPageAnim() {
  var $landingPage = $("#home-section")
  var width = $(window).width()
  let animFinishMargin = ''
  if (width > 610) {
    animFinishMargin = '60px'
  } else {
    animFinishMargin = '30px'
  }
  setTimeout(function() {
    $landingPage.animate({
      opacity: '1.0',
      marginTop: animFinishMargin
    }, 500, "swing")
  }, 400)
}

// Temporarily enlarges the contact info when 'Contact' button is clicked in the nav menu
function highlightContacts() {
  $contactsContainer = $("#contacts_container")
  $contactsContainer.addClass('highlight-contact-info')
  $contactsContainer.removeClass('unhighlight-contact-info')
  setTimeout(() => {
    $contactsContainer.removeClass('highlight-contact-info')
    $contactsContainer.addClass('unhighlight-contact-info')
  }, 800)
}

$('.experience-cell').click(function() {
  $(this).toggleClass('view-experience-cell', 500)
  $(this).find('.experience-category-title').toggleClass('experience-category-title-viewed', 500)
})
