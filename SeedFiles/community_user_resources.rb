org_community = Organization.create_if_not_exists(
  id:   1,
  name: 'VIACode',
)
user_community = User.create_or_update(
  id:              2,
  login:           'sales@VIAcode.com',
  firstname:       'VIAcode',
  lastname:        'Sales',
  email:           'sales@VIAcode.com',
  password:        '',
  active:          true,
  roles:           [ Role.find_by(name: 'Customer') ],
  organization_id: org_community.id,
)

UserInfo.current_user_id = user_community.id

if Ticket.count.zero?
  ticket = Ticket.create!(
    group_id:    Group.find_by(name: 'Users').id,
    customer_id: User.find_by(login: 'sales@VIAcode.com').id,
    title:       'Welcome to VIAcode !',
  )
  Ticket::Article.create!(
    ticket_id: ticket.id,
    type_id:   Ticket::Article::Type.find_by(name: 'phone').id,
    sender_id: Ticket::Article::Sender.find_by(name: 'Customer').id,
    from:      'VIAcode Sales <sales@viacode.com>',
    body:      'Welcome!

  Thank you for choosing VIAcode.

  ',
    internal:  false,
  )
end

UserInfo.current_user_id = 1
