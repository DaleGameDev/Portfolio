/*
Bullet Continuous Collision Detection and Physics Library
Copyright (c) 2003-2006 Erwin Coumans  http://continuousphysics.com/Bullet/

This software is provided 'as-is', without any express or implied warranty.
In no event will the authors be held liable for any damages arising from the use of this software.
Permission is granted to anyone to use this software for any purpose, 
including commercial applications, and to alter it and redistribute it freely, 
subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/



#define CUBE_HALF_EXTENTS 1

#define EXTRA_HEIGHT 1.f

#include "INM377ProjTemplateForces.h"
#include "GlutStuff.h"
#include "GLDebugFont.h"

///btBulletDynamicsCommon.h is the main Bullet include file, contains most common include files.
#include "btBulletDynamicsCommon.h"

#include <stdio.h> //printf debugging
#include "GLDebugDrawer.h"


#if 0
extern btAlignedObjectArray<btVector3> debugContacts;
extern btAlignedObjectArray<btVector3> debugNormals;
#endif 

static GLDebugDrawer	sDebugDrawer;


INM377ProjTemplate::INM377ProjTemplate()
:m_ccdMode(USE_CCD)
{
	setDebugMode(btIDebugDraw::DBG_DrawText+btIDebugDraw::DBG_NoHelpText);
	setCameraDistance(btScalar(40.));
}


void INM377ProjTemplate::clientMoveAndDisplay()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT); 

	//simple dynamics world doesn't handle fixed-time-stepping
	//float ms = getDeltaTimeMicroseconds();
	
	///step the simulation
	if (m_dynamicsWorld)
	{
		m_dynamicsWorld->stepSimulation(1./60.,0);//ms / 1000000.f);
		//optional but useful: debug drawing
		m_dynamicsWorld->debugDrawWorld();
	}
		
	renderme(); 

//	displayText();
#if 0
	for (int i=0;i<debugContacts.size();i++)
	{
		getDynamicsWorld()->getDebugDrawer()->drawContactPoint(debugContacts[i],debugNormals[i],0,0,btVector3(1,0,0));
	}
#endif

	glFlush();

	swapBuffers();

}


void INM377ProjTemplate::displayText()
{
	int lineWidth=440;
	int xStart = m_glutScreenWidth - lineWidth;
	int yStart = 20;

	if((getDebugMode() & btIDebugDraw::DBG_DrawText)!=0)
	{
		setOrthographicProjection();
		glDisable(GL_LIGHTING);
		glColor3f(0, 0, 0);
		char buf[124];
		
		glRasterPos3f(xStart, yStart, 0);
		switch (m_ccdMode)
		{
		case USE_CCD:
			{
				sprintf_s(buf,"Predictive contacts and motion clamping");
				break;
			}
		case USE_NO_CCD:
			{
				sprintf_s(buf,"CCD handling disabled");
				break;
			}
		default:
			{
				sprintf_s(buf,"unknown CCD setting");
			};
		};

		GLDebugDrawString(xStart,20,buf);
		glRasterPos3f(xStart, yStart, 0);
		sprintf_s(buf,"Press 'p' to change CCD mode");
		yStart+=20;
		GLDebugDrawString(xStart,yStart,buf);
		glRasterPos3f(xStart, yStart, 0);
		sprintf_s(buf,"Press '.' or right mouse to shoot bullets");
		yStart+=20;
		GLDebugDrawString(xStart,yStart,buf);
		glRasterPos3f(xStart, yStart, 0);
		sprintf_s(buf,"space to restart, h(elp), t(ext), w(ire)");
		yStart+=20;
		GLDebugDrawString(xStart,yStart,buf);
		
		resetPerspectiveProjection();
		glEnable(GL_LIGHTING);
	}	

}



void INM377ProjTemplate::displayCallback(void) {

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT); 
	
	renderme();

	displayText();

	//optional but useful: debug drawing to detect problems
	if (m_dynamicsWorld)
	{
		m_dynamicsWorld->debugDrawWorld();
	}
#if 0
	for (int i=0;i<debugContacts.size();i++)
	{
		getDynamicsWorld()->getDebugDrawer()->drawContactPoint(debugContacts[i],debugNormals[i],0,0,btVector3(1,0,0));
	}
#endif

	glFlush();
	swapBuffers();
}

//Global declaration of the AI class so i can use it inside the call back function 
AI ai;

//This code is a mixture of tutorial code and my own
void MyTickCallback(btDynamicsWorld *world, btScalar timeStep) {

	
		world->clearForces();
		for (int i = 0;i < static_cast<INM377ProjTemplate *>(world->getWorldUserInfo())->boids.size();i++) {
			//btRigidBody* body0 = static_cast<INM377ProjTemplate *>(world->getWorldUserInfo())->body000;
			btRigidBody* body0 = static_cast<INM377ProjTemplate *>(world->getWorldUserInfo())->boids[i];
			bool scared = false;
			btScalar mass = body0->getInvMass();
			btVector3 vel = body0->getLinearVelocity();
			btVector3 gravity = body0->getGravity();
			btTransform mat = body0->getWorldTransform();
		
			btVector3 dir = ai.think(mat.getOrigin(),scared);
			//speed of the boids 
			btScalar speed = 25;
			//checks the flee state and speeds up as required 
			if (ai.getFlee()) {
				speed = 40;
			}

			if (scared) {
				speed = 100;
			}
			
			btVector3 thrust = speed * dir;
			btVector3 drag = -3 * vel;
			btVector3 lift = -gravity;
			
			

			btVector3 result = thrust + lift + gravity + drag;
			body0->applyCentralForce(result);

			btTransform transform(body0->getOrientation());
			btVector3 heading = body0->getLinearVelocity();
			heading = heading.safeNormalize();		
			
			const float torqueMag = 2.5f;
			btVector3 front = transform * btVector3(0, 0, 1);	
			const btVector3 up(0, 1, 0);
			btVector3 top = transform * up;
			btVector3 alignTorque = torqueMag * front.cross(heading);
			btVector3 flattenTorque = torqueMag * top.cross(up);
			btVector3 angular = body0->getAngularVelocity();

			//To control the momentum the angular velocity is removed 
			body0->applyTorque(alignTorque-angular);
			body0->applyTorque(flattenTorque - angular);

			
		}
		ai.schoolThink();
}

void	INM377ProjTemplate::initPhysics()
{
	ai = AI();

	setTexturing(true);
	setShadows(false);

	setCameraDistance(150.f);


	// init world
	m_collisionConfiguration = new btDefaultCollisionConfiguration();
	m_dispatcher = new btCollisionDispatcher(m_collisionConfiguration);
	btVector3 worldMin(-1000, -1000, -1000);
	btVector3 worldMax(1000, 1000, 1000);
	m_overlappingPairCache = new btAxisSweep3(worldMin, worldMax);

	m_constraintSolver = new btSequentialImpulseConstraintSolver();

	btDiscreteDynamicsWorld* wp = new btDiscreteDynamicsWorld(m_dispatcher, m_overlappingPairCache, m_constraintSolver, m_collisionConfiguration);


	//	wp->getSolverInfo().m_numIterations = 20; // default is 10
	m_dynamicsWorld = wp;
	m_dynamicsWorld->setInternalTickCallback(MyTickCallback, static_cast<void *>(this), true);
	

	///create a few basic rigid bodies
	btBoxShape* box = new btBoxShape(btVector3(btScalar(150.),btScalar(1.),btScalar(150.)));
//	box->initializePolyhedralFeatures();
	btCollisionShape* groundShape = box;

//	btCollisionShape* groundShape = new btStaticPlaneShape(btVector3(0,1,0),50);
	
	m_collisionShapes.push_back(groundShape);
	//m_collisionShapes.push_back(new btCylinderShape (btVector3(CUBE_HALF_EXTENTS,CUBE_HALF_EXTENTS,CUBE_HALF_EXTENTS)));
	m_collisionShapes.push_back(new btBoxShape (btVector3(CUBE_HALF_EXTENTS,CUBE_HALF_EXTENTS,CUBE_HALF_EXTENTS)));

	btTransform groundTransform;
	groundTransform.setIdentity();
	groundTransform.setOrigin(btVector3(0,-10,0));

	btSphereShape* sphere = new btSphereShape(btScalar(10));
	btCollisionShape* ballShape = sphere;
	m_collisionShapes.push_back(sphere);

	//We can also use DemoApplication::localCreateRigidBody, but for clarity it is provided here:
	{
		btScalar mass(0.);

		//rigidbody is dynamic if and only if mass is non zero, otherwise static
		bool isDynamic = (mass != 0.f);

		btVector3 localInertia(0,0,0);
		if (isDynamic)
			groundShape->calculateLocalInertia(mass,localInertia);

		//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
		btDefaultMotionState* myMotionState = new btDefaultMotionState(groundTransform);
		btRigidBody::btRigidBodyConstructionInfo rbInfo(mass,myMotionState,groundShape,localInertia);
		btRigidBody* body = new btRigidBody(rbInfo);
		body->setFriction(0.5);

		m_dynamicsWorld->addRigidBody(body);
	}


	{
		
		//instead of using a polymesh the boid is a convexhullshape
		btConvexHullShape* con = new btConvexHullShape(0, 0, sizeof(btVector3));
		btVector3 one(-1, 0.5, -1);
		btVector3 two(1, 0.5, -1); 
		btVector3 three(-1, -0.5, -1); 
		btVector3 four(1, -0.5, -1);
		btVector3 five(0, 0, 2);
	

		 con->addPoint(one,true);
		 con->addPoint(two, true);
		 con->addPoint(three, true);
		 con->addPoint(four, true);
		 con->addPoint(five, true);
	
		 
		 btCollisionShape* colShape = con;
		//btCollisionShape* colShape = new btBoxShape(btVector3(5, 3, 5));
		m_collisionShapes.push_back(colShape);
		btTransform trans;
		trans.setIdentity();
		btCollisionShape* shape = m_collisionShapes[3];
		boids.push_back(body000);boids.push_back(body001);boids.push_back(body002);boids.push_back(body003);
		boids.push_back(body004);boids.push_back(body005);boids.push_back(body006);

		//creating the 7 boids in the world
		for (int i = 0; i < boids.size();i++) {
			int x = (rand() % 50)-25;
			int y = (rand() % 20) - 10;
			int z = (rand() % 50) - 25;
			btVector3 pos(x, 20+y, z);
			trans.setOrigin(pos);
			btScalar mass(1.0f);
			// shape->calculateLocalInertia(mass, LocalInertia);
			boids[i] = localCreateRigidBody(mass, trans, shape);
			boids[i]->setAnisotropicFriction(shape->getAnisotropicRollingFrictionDirection(), btCollisionObject::CF_ANISOTROPIC_ROLLING_FRICTION);
			boids[i]->setFriction(0.5);
			//		body000->setLinearVelocity(btVector3(1, 0, 0));
			boids[i]->activate(true);
		}
	}

	{
		//creating the enemies to be avoided by the boids 
		spheres.push_back(sphere00);spheres.push_back(sphere01);spheres.push_back(sphere02);
		spheres.push_back(sphere03);spheres.push_back(sphere04);spheres.push_back(sphere05);
		spheres.push_back(sphere06);spheres.push_back(sphere07);
		btCollisionShape* shape = m_collisionShapes[2];
		btTransform trans;
		trans.setIdentity();
		for (int i = 0; i < spheres.size();i++) {
		
			int x = (rand() % 200) - 100;
			int y = (rand() % 80) +20;
			int z = (rand() % 200) - 100;
			btVector3 pos(x, y, z);
			trans.setOrigin(pos);
			btScalar mass(0.0f);
			spheres[i] = localCreateRigidBody(mass, trans, shape);
			spheres[i]->setAnisotropicFriction(shape->getAnisotropicRollingFrictionDirection(), btCollisionObject::CF_ANISOTROPIC_ROLLING_FRICTION);
			spheres[i]->setFriction(0.5);
			spheres[i]->activate(true);
			spheres[i]->isStaticObject();
			ai.enemyPositions.push_back(pos);
		}
	}

}

void	INM377ProjTemplate::clientResetScene()
{
	exitPhysics();
	initPhysics();
}

void INM377ProjTemplate::keyboardCallback(unsigned char key, int x, int y)
{
	if (key=='p')
	{
		switch (m_ccdMode)
		{
			case USE_CCD:
			{
				m_ccdMode = USE_NO_CCD;
				break;
			}
			case USE_NO_CCD:
			default:
			{
				m_ccdMode = USE_CCD;
			}
		};
		clientResetScene();
	} else
	{
		DemoApplication::keyboardCallback(key,x,y);
	}
}


void	INM377ProjTemplate::shootBox(const btVector3& destination)
{

	if (m_dynamicsWorld)
	{
		float mass = 1.f;
		btTransform startTransform;
		startTransform.setIdentity();
		btVector3 camPos = getCameraPosition();
		startTransform.setOrigin(camPos);

		setShootBoxShape ();


		btRigidBody* body = this->localCreateRigidBody(mass, startTransform,m_shootBoxShape);
		body->setLinearFactor(btVector3(1,1,1));
		//body->setRestitution(1);

		btVector3 linVel(destination[0]-camPos[0],destination[1]-camPos[1],destination[2]-camPos[2]);
		linVel.normalize();
		linVel*=m_ShootBoxInitialSpeed;

		body->getWorldTransform().setOrigin(camPos);
		body->getWorldTransform().setRotation(btQuaternion(0,0,0,1));
		body->setLinearVelocity(linVel);
		body->setAngularVelocity(btVector3(0,0,0));
		body->setContactProcessingThreshold(1e30);

		///when using m_ccdMode, disable regular CCD
		if (m_ccdMode==USE_CCD)
		{
			body->setCcdMotionThreshold(CUBE_HALF_EXTENTS);
			body->setCcdSweptSphereRadius(0.4f);
		}
		
	}
}




void	INM377ProjTemplate::exitPhysics()
{

	//cleanup in the reverse order of creation/initialization

	//remove the rigidbodies from the dynamics world and delete them
	int i;
	for (i=m_dynamicsWorld->getNumCollisionObjects()-1; i>=0 ;i--)
	{
		btCollisionObject* obj = m_dynamicsWorld->getCollisionObjectArray()[i];
		btRigidBody* body = btRigidBody::upcast(obj);
		if (body && body->getMotionState())
		{
			delete body->getMotionState();
		}
		m_dynamicsWorld->removeCollisionObject( obj );
		delete obj;
	}

	//delete collision shapes
	for (int j=0;j<m_collisionShapes.size();j++)
	{
		btCollisionShape* shape = m_collisionShapes[j];
		delete shape;
	}
	m_collisionShapes.clear();

	delete m_dynamicsWorld;
	
	
//	delete m_solver;
	
//	delete m_broadphase;
	
//	delete m_dispatcher;

//	delete m_collisionConfiguration;

	
}




