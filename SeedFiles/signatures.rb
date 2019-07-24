Signature.create_if_not_exists(
  id:            1,
  name:          'default',
  body:          '
  #{user.firstname} #{user.lastname}

--
 VIAcode Incident Management
 Email: support@viacode.com - Web: http://www.viacode.com/
--'.text2html,
  updated_by_id: 1,
  created_by_id: 1
)
